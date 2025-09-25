using ETLProyecto.Data;
using ETLProyecto.Models;
using System.Data;
using System.Data.SqlClient;

namespace ETLProyecto.Services
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
            var lista = details.ToList();
            if (!lista.Any()) return 0;

            using var conn = _dbFactory.CreateConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, TotalPrice)
                                    VALUES (@orderId, @productId, @quantity, @totalPrice);";

                cmd.Parameters.Add(new SqlParameter("@orderId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@totalPrice", SqlDbType.Decimal) { Precision = 10, Scale = 2 });

                int inserted = 0;
                foreach (var d in lista)
                {
                    cmd.Parameters["@orderId"].Value = d.OrderID;
                    cmd.Parameters["@productId"].Value = d.ProductID;
                    cmd.Parameters["@quantity"].Value = d.Quantity;
                    cmd.Parameters["@totalPrice"].Value = d.TotalPrice;

                    await cmd.ExecuteNonQueryAsync();
                    inserted++;
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
