using ETLProyecto.Data;
using ETLProyecto.Models;
using System.Data;
using System.Data.SqlClient;

namespace ETLProyecto.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IDbConnectionFactory _dbFactory;

        public ClienteService(IDbConnectionFactory dbFactory) => _dbFactory = dbFactory;

        public async Task<int> InsertClientesAsync(IEnumerable<Cliente> clientes)
        {
            var lista = clientes.ToList();
            if (!lista.Any()) return 0;

            using var conn = _dbFactory.CreateConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = @"INSERT INTO Clientes 
                                    (CustomerID, FirstName, LastName, Email, Phone, City, Country, FuenteID)
                                    VALUES (@customerId, @firstName, @lastName, @email, @phone, @city, @country, @fuenteId);";

                cmd.Parameters.Add(new SqlParameter("@customerId", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 100));
                cmd.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@city", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@country", SqlDbType.NVarChar, 50));
                cmd.Parameters.Add(new SqlParameter("@fuenteId", SqlDbType.Int) { Value = DBNull.Value });

                int inserted = 0;
                foreach (var c in lista)
                {
                    cmd.Parameters["@customerId"].Value = c.CustomerID;
                    cmd.Parameters["@firstName"].Value = c.FirstName;
                    cmd.Parameters["@lastName"].Value = c.LastName;
                    cmd.Parameters["@email"].Value = (object?)c.Email ?? DBNull.Value;
                    cmd.Parameters["@phone"].Value = (object?)c.Phone ?? DBNull.Value;
                    cmd.Parameters["@city"].Value = (object?)c.City ?? DBNull.Value;
                    cmd.Parameters["@country"].Value = (object?)c.Country ?? DBNull.Value;
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
