using System.Data.SqlClient;

namespace TickAndDashDAL.DAL
{
    public abstract class BaseDAL
    {
        private readonly string _connectionString;

        public BaseDAL()
        {
            _connectionString = "Data Source=.;Initial Catalog=TickAndDash;User ID=DevLogin;Password=xxx";
        }

        public SqlConnection GetTickAndDashConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}
