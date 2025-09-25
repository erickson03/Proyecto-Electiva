using ETLProyecto.Data;
using ETLProyecto.Models;
using System.Data;
using System.Data.SqlClient;

namespace ETLProyecto.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IDbConnectionFactory _dbFactory;

        public ProductoService(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<int> InsertProductosAsync(IEnumerable<Producto> productos)
        {
            var lista = productos.ToList();
            if (!lista.Any()) return 0;

            using var conn = _dbFactory.CreateConnection();
            await conn.OpenAsync();

            using var tran = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = @"INSERT INTO Productos 
                                    (ProductID, ProductName, Category, Price, Stock, FuenteID)
                                    VALUES (@productId, @productName, @category, @price, @stock, @fuenteId);";

                cmd.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@productName", SqlDbType.NVarChar, 100));
                cmd.Parameters.Add(new SqlParameter("@category", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Precision = 10, Scale = 2 });
                cmd.Parameters.Add(new SqlParameter("@stock", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@fuenteId", SqlDbType.Int) { Value = DBNull.Value });

                int inserted = 0;
                foreach (var p in lista)
                {
                    cmd.Parameters["@productId"].Value = p.ProductID;
                    cmd.Parameters["@productName"].Value = p.ProductName;
                    cmd.Parameters["@category"].Value = (object?)p.Category ?? DBNull.Value;
                    cmd.Parameters["@price"].Value = p.Price;
                    cmd.Parameters["@stock"].Value = p.Stock;
                    cmd.Parameters["@fuenteId"].Value = DBNull.Value;

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
