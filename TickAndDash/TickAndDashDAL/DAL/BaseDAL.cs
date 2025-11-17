using System;
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
                System.Diagnostics.Debug.WriteLine($"BaseDAL: Connection string from IConfiguration: {(string.IsNullOrEmpty(_connectionString) ? "NOT FOUND" : "FOUND")}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("BaseDAL: IConfiguration is null!");
            }
            
            // If not found in configuration, try environment variable (for Azure)
            // Azure App Service uses double underscore (__) for nested configuration
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings__TickAndDash");
                System.Diagnostics.Debug.WriteLine($"BaseDAL: Connection string from ConnectionStrings__TickAndDash: {(string.IsNullOrEmpty(_connectionString) ? "NOT FOUND" : "FOUND")}");
            }
            
            // Also try with single underscore (some configurations use this)
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings:TickAndDash");
                System.Diagnostics.Debug.WriteLine($"BaseDAL: Connection string from ConnectionStrings:TickAndDash: {(string.IsNullOrEmpty(_connectionString) ? "NOT FOUND" : "FOUND")}");
            }
            
            // Try direct environment variable
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = System.Environment.GetEnvironmentVariable("TickAndDash_ConnectionString");
                System.Diagnostics.Debug.WriteLine($"BaseDAL: Connection string from TickAndDash_ConnectionString: {(string.IsNullOrEmpty(_connectionString) ? "NOT FOUND" : "FOUND")}");
            }
            
            // Fallback to hardcoded connection string (for local development only)
            // This should NEVER be used in production!
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = "Data Source=.;Initial Catalog=TickAndDash;User ID=DevLogin;Password=xxx";
                System.Diagnostics.Debug.WriteLine("BaseDAL: Using FALLBACK connection string (local development)");
            }
            
            // Log connection string status (without exposing sensitive data)
            if (string.IsNullOrEmpty(_connectionString))
            {
                System.Diagnostics.Debug.WriteLine("BaseDAL: ERROR - Connection string is NULL or EMPTY!");
            }
            else
            {
                // Log first part of connection string (server name) for debugging
                var serverPart = _connectionString.Contains("Data Source=") 
                    ? _connectionString.Substring(_connectionString.IndexOf("Data Source=") + 12).Split(';')[0]
                    : "Unknown";
                System.Diagnostics.Debug.WriteLine($"BaseDAL: Connection string is set. Server: {serverPart}");
            }
        }

        public SqlConnection GetTickAndDashConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("BaseDAL.GetTickAndDashConnection: ERROR - Connection string is null or empty!");
                    throw new InvalidOperationException("Connection string is not configured. Please set ConnectionStrings:TickAndDash in appsettings.json or Azure App Service Configuration.");
                }
                
                var connection = new SqlConnection(_connectionString);
                System.Diagnostics.Debug.WriteLine("BaseDAL.GetTickAndDashConnection: SqlConnection created successfully");
                return connection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BaseDAL.GetTickAndDashConnection: ERROR creating connection - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
