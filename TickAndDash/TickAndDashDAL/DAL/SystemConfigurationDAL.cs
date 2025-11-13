using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class SystemConfigurationDAL : BaseDAL, ISystemConfigurationDAL
    {
        public SystemConfigurationDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private const string SystemConfigurationTable = "SystemConfiguration";


        public async Task<List<SystemConfiguration>> GetAllAsync()
        {
            string query = $@"SELECT * FROM systemConfiguration";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<SystemConfiguration>(query);
                return result.ToList();
            }
        }

        public async Task<string> GetSettingValueByKeyAsync(SettingKeyEnum settingKey)
        {
            string query = $@"SELECT SettingValue FROM systemConfiguration
                                where SettingKey = @settingKey;
                            ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<string>(query, new { settingKey = settingKey.ToString() });
                return result.FirstOrDefault();
            }
        }

        public bool Update(SystemConfiguration systemConfiguration)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"UPDATE {SystemConfigurationTable} 
                                    SET SettingValue = @SettingValue
                                    WHERE Id = @Id";
                return connection.Execute(sql,
                    new
                    {
                        systemConfiguration.SettingValue,
                        systemConfiguration.Id
                    }) > 0;
            }
        }
    }
}
