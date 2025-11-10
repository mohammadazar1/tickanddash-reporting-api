using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class SitesDAL : BaseDAL, ISitesDAL
    {
        private readonly string _defaultschema = "dbo";
        private readonly string _sitesTable = "Sites";
        private readonly string _sitesTypelookup = "SitesTypelookup";
        private readonly string _sitesName = "SitesNames";

        public SitesDAL() : base()
        {

        }

        public bool Insert(Site site)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"INSERT INTO {_sitesTable} (Name, SitesTypeLookupId, Description, Latitude, Longitude, IsActive)
                                VALUES(@Name, @SitesTypeLookupId, @Description, @Latitude, @Longitude, @IsActive)";

                return connection.Execute(sql,
                    new
                    {
                        site.Name,
                        site.SitesTypeLookupId,
                        site.Description,
                        site.Latitude,
                        site.Longitude,
                        site.IsActive
                    }) > 0;
            }
        }

        public bool Delete(int Id)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"DELETE {_sitesTable} WHERE Id = @Id";

                return connection.Execute(sql, new { Id }) > 0;
            }
        }

        public async Task<List<Site>> GetAllSitesThasHasMainPickupStationAsync(bool isActive, string language)
        {
            language = language.ToLower();
            string query = $@"SELECT distinct {(language == "ar" ? "s." : "sn.")}Name ,s.Id
                                FROM Sites s, SitesNames sn, PickupStations ps
                                WHERE  s.Id = sn.SiteId AND
                                      s.Id = ps.SiteId and ps.PickupTypeId = {(int)PickupStationsLookupEnum.Main}
                                      and s.IsActive = 1
                                      and ps.IsActive = 1
                                    {(language == "ar" ? "" : $"AND sn.Language = '{language}'")} 
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<Site>(query, new { isActive });
                return result.ToList();
            }
        }

        public async Task<bool> IsSiteActiveAsync(int siteId)
        {
            string query = $@"SELECT Id  FROM[TickAndDash].[{_defaultschema}].[{_sitesTable}] 
                                where id = @siteId;";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { siteId });
                return result.FirstOrDefault() > 0;
            }
        }

        public bool Update(Site site)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"UPDATE {_sitesTable} 
                               SET Name = @Name, SitesTypeLookupId = @SitesTypeLookupId, 
                                Description = @Description, Latitude = @Latitude, 
                                Longitude = @Longitude, IsActive = @IsActive
                               WHERE Id = @Id ";

                return connection.Execute(sql,
                    new
                    {
                        site.Name,
                        site.SitesTypeLookupId,
                        site.Description,
                        site.Latitude,
                        site.Longitude,
                        site.IsActive,
                        site.Id
                    }) > 0;
            }
        }

        public IList<Site> GetAll()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"SELECT * FROM {_sitesTable} s, SitesTypelookup stl
                                WHERE s.SitesTypeLookupId = stl.Id";
                return connection.Query<Site, SiteLookup, Site>(sql, (site, siteLookup) =>
                {
                    site.SiteLookup = siteLookup;
                    return site;
                }).ToList();
            }
        }

        public IList<SiteLookup> GetAllLookupSites()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"SELECT * FROM {_sitesTypelookup}";
                return connection.Query<SiteLookup>(sql).ToList();
            }
        }
    }
}
