using Dapper;
using System;
using System.Linq;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class CityDAL : BaseDAL, ICityDAL
    {
        public CityDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private string _cityTable = "cities";
        private string _defaultschema = "dbo";
        public CityDAL()
        {
        }

        public Site GetSitesByName(string name)
        {
            throw new NotImplementedException();

            string query = $@"SELECT * FROM[TickAndDash].[{_defaultschema}].[{_cityTable}] 
                                where Name like CONCAT('%',@name,'%');
                            ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var Sites = sqlConnection.Query<Site>(query, new { name }).FirstOrDefault();
                return Sites;
            }
        }
    }
}
