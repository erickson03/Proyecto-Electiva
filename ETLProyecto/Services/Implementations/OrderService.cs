using System.Data;
using ETLProyecto.Data.Connections;
using ETLProyecto.Models;
using ETLProyecto.Services.Interfaces;
using Microsoft.Data.SqlClient; 

namespace ETLProyecto.Services.Implementations
{
    public class OrderService : IOrderService   // 👈 implementar interfaz
    {
        private readonly IDbConnectionFactory _dbFactory;

        public OrderService(IDbConnectionFactory dbFactory) => _dbFactory = dbFactory;

        public async Task<int> InsertOrdersAsync(IEnumerable<Order> orders)
        {
            var lista = orders
                .Where(o => o.OrderID > 0 && o.CustomerID > 0 && o.OrderDate != default)
                .ToList();

            if (!lista.Any()) return 0;

            using var conn = _dbFactory.CreateConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = @"IF EXISTS (SELECT 1 FROM Clientes WHERE CustomerID = @customerId)
                                    BEGIN
                                        IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderID = @orderId)
                                        INSERT INTO Orders (OrderID, CustomerID, OrderDate, Status)
                                        VALUES (@orderId, @customerId, @orderDate, @status);
                                    END";

                cmd.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@customerId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@orderDate", SqlDbType.Date));
                cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50));

                int inserted = 0;
                foreach (var o in lista)
                {
                    cmd.Parameters["@orderId"].Value = o.OrderID;
                    cmd.Parameters["@customerId"].Value = o.CustomerID;
                    cmd.Parameters["@orderDate"].Value = o.OrderDate;
                    cmd.Parameters["@status"].Value = (object?)o.Status ?? DBNull.Value;

                    inserted += await cmd.ExecuteNonQueryAsync();
                }

                await tran.CommitAsync();
                return inserted;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        public async Task<int> InsertOrderDetailsAsync(IEnumerable<OrderDetail> details)
        {
            var lista = details.Where(d => d.OrderID > 0 && d.ProductID > 0 && d.Quantity > 0).ToList();
            if (!lista.Any()) return 0;

            using var conn = _dbFactory.CreateConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                int inserted = 0;
                foreach (var d in lista)
                {
                    // Obtener precio real del producto
                    decimal price;
                    using (var priceCmd = conn.CreateCommand())
                    {
                        priceCmd.Transaction = tran;
                        priceCmd.CommandText = "SELECT Price FROM Productos WHERE ProductID = @productId";
                        priceCmd.Parameters.Add(new SqlParameter("@productId", d.ProductID));
                        var result = await priceCmd.ExecuteScalarAsync();
                        if (result == null) continue;
                        price = (decimal)result;
                    }

                    // Calcular TotalPrice
                    d.TotalPrice = d.Quantity * price;

                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = @"IF EXISTS (SELECT 1 FROM Orders WHERE OrderID = @orderId)
                                        BEGIN
                                            INSERT INTO OrderDetails (OrderID, ProductID, Quantity, TotalPrice)
                                            VALUES (@orderId, @productId, @quantity, @totalPrice);
                                        END";

                    cmd.Parameters.Add(new SqlParameter("@orderId", d.OrderID));
                    cmd.Parameters.Add(new SqlParameter("@productId", d.ProductID));
                    cmd.Parameters.Add(new SqlParameter("@quantity", d.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@totalPrice", d.TotalPrice));

                    inserted += await cmd.ExecuteNonQueryAsync();
                }

                await tran.CommitAsync();
                return inserted;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
            finally
            {
                await conn.CloseAsync();
            }
        }
    }
}
