using System.Data.SqlClient;

namespace ETLProyecto.Data
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
