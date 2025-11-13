using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TickAndDashDAL.DAL
{
    public abstract class BaseDAL
    {
        private readonly string _connectionString;

        public BaseDAL(IConfiguration configuration = null)
        {
            // Try to get connection string from configuration first
            if (configuration != null)
            {
                _connectionString = configuration.GetConnectionString("TickAndDash");
            }
            
            // If not found in configuration, try environment variable (for Azure)
            // Azure App Service uses double underscore (__) for nested configuration
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings__TickAndDash");
            }
            
            // Also try with single underscore (some configurations use this)
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings:TickAndDash");
            }
            
            // Try direct environment variable
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("TickAndDash_ConnectionString");
            }
            
            // Fallback to hardcoded connection string (for local development only)
            // This should NEVER be used in production!
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = "Data Source=.;Initial Catalog=TickAndDash;User ID=DevLogin;Password=xxx";
            }
        }

        public SqlConnection GetTickAndDashConnection()
        {
            return new SqlConnection(_connectionString);
        }

    }
}
