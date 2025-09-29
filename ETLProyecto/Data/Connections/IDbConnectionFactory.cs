using System.Data.SqlClient;

namespace ETLProyecto.Data.Connections
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
