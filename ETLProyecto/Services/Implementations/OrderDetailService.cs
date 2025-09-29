using ETLProyecto.Data.Connections;
using ETLProyecto.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ETLProyecto.Services.Implementations
{
    public class OrderDetailService
    {
        private readonly IDbConnectionFactory _dbFactory;

        public OrderDetailService(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
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
                    // Recuperar precio del producto desde BD
                    decimal price;
                    using (var priceCmd = conn.CreateCommand())
                    {
                        priceCmd.Transaction = tran;
                        priceCmd.CommandText = "SELECT Price FROM Productos WHERE ProductID = @productId";
                        priceCmd.Parameters.Add(new SqlParameter("@productId", d.ProductID));
                        var result = await priceCmd.ExecuteScalarAsync();
                        if (result == null) continue; // producto no existe
                        price = (decimal)result;
                    }

                    // Calcular total
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
