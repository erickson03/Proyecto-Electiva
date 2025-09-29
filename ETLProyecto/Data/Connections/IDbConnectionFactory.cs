using Microsoft.Data.SqlClient;

namespace ETLProyecto.Data.Connections
{
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
