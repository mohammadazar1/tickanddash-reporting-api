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
    public class TransItinerariesDAL : BaseDAL, ITransItinerariesDAL
    {
        public TransItinerariesDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private string _itineraryTable = "Transportation_Itineraries";
        private string _defaultschema = "dbo";
        public TransItinerariesDAL() : base()
        {

        }

        public IList<Transportation_Itineraries> GetAll(bool? isActive = null)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"SELECT * FROM {_itineraryTable} it, Sites fromSite, Sites toSites 
                                WHERE it.FromSiteId = fromSite.Id AND it.TowardSiteId = toSites.Id
                                        {(isActive != null ? " AND it.IsActive = 1" : "")}";

                var result = connection.Query<Transportation_Itineraries, Site, Site, Transportation_Itineraries>(sql,
                    (trans, fromSite, toSite) =>
                    {
                        trans.FromSite = fromSite;
                        trans.TowardSite = toSite;
                        return trans;
                    },
                    splitOn: "Id,Id");

                return result.ToList();
            }
        }

        public Transportation_Itineraries GetById(int transItineraryId)
        {
            using (var connectino = GetTickAndDashConnection())
            {
                var query = $@"SELECT * FROM {_itineraryTable} WHERE ID = {transItineraryId}";

                return connectino.Query<Transportation_Itineraries>(query).FirstOrDefault();
            }
        }

        public async Task<List<Site>> GetSitesToVisitGoFromSpecificSiteAsync(int siteId, string language)
        {
            List<Site> sites = new List<Site>();
            // already one way iternary 
            string queryFrom = $@"
                                SELECT distinct {(language != "ar" ? "sn.SiteId as 'Id', sn.Name " : "s.Id, s.Name")}  
                                FROM[TickAndDash].[{_defaultschema}].[{_itineraryTable}] iter  ,Sites s
                                {(language != "ar" ? ", SitesNames sn" : "")}
                                WHERE iter.TowardSiteId = s.Id
                                 {(language != "ar" ? "and sn.SiteId = S.Id AND sn.Language = '" + language + "'" : "")} 
                                and iter.FromSiteId = @siteId
                                and iter.IsActive = 1
                                and s.IsActive = 1
                                                            ";

            // already two way iternary 
            string queryTo = $@"SELECT distinct {(language != "ar" ? "sn.SiteId as 'Id', sn.Name " : "s.Id, s.Name")}  
                                FROM[TickAndDash].[{_defaultschema}].[{_itineraryTable}] iter  ,  Sites s
                                 {(language != "ar" ? ", SitesNames sn" : "")}
                                WHERE iter.FromSiteId = s.Id  
                                 {(language != "ar" ? "and sn.SiteId = S.Id AND sn.Language = '" + language + "'" : "")} 
                                AND TowardSiteId = @siteId
                                AND ItineraryTypeLookupId = {(int)ItineraryTypeLookupEnum.Two_Way}
                                AND iter.IsActive = 1
                                AND s.IsActive = 1                            ";

            string query = queryFrom + queryTo;

            using (var sqlConnection = GetTickAndDashConnection())
            {
                using (var multi = await sqlConnection.QueryMultipleAsync(query, new { siteId }))
                {
                    var fromSitesResult = await multi.ReadAsync<Site>();
                    var toSitesResult = await multi.ReadAsync<Site>();

                    var fromSites = fromSitesResult.ToList();
                    var toSites = toSitesResult.ToList();

                    if (fromSites != null && fromSites.Any())
                    {
                        sites.AddRange(fromSites);
                    }

                    if (toSites != null && toSites.Any())
                    {
                        sites.AddRange(toSites);
                    }
                }
            }

            return sites;
        }

        public async Task<int> GetTransItinerariesBySitesEndPointsAsync(int fromSiteId, int towrdSiteId)
        {
            string query = $@"SELECT id FROM Transportation_Itineraries iter
                              where
                              ((iter.FromSiteId = @fromSiteId and iter.TowardSiteId = @towrdSiteId)
                              or (iter.FromSiteId = @towrdSiteId and iter.TowardSiteId = @fromSiteId and iter.ItineraryTypeLookupId ={(int)ItineraryTypeLookupEnum.Two_Way}))
                              and iter.IsActive = 1";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { fromSiteId, towrdSiteId });
                return result.FirstOrDefault();
            }
        }

        public async Task<decimal> GetTransItineraryPriceByPickupStation(int psId)
        {
            string query = @"SELECT price FROM PickupStations ps, Transportation_Itineraries trans 
                                      WHERE  ps.TransItineraryId = trans.Id
                                             and ps.id = @psId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<decimal>(query, new { psId });
                return res.FirstOrDefault();
            }
        }

        public async Task<List<Transportation_Itineraries>> GetTransportationsIternaryPrices(int transId, string language)
        {
            string query = $@" SELECT Id, Price,
                                {(language == "ar"? "Name":"NameEn AS 'Name'")}
                                FROM Transportation_Itineraries 
                                WHERE IsActive = 1";

            if (transId > 0)
            {
                string.Concat(query, " Where id = @transId ");
            }

            using (var connection = GetTickAndDashConnection())
            {
                var res = await connection.QueryAsync<Transportation_Itineraries>(query, new { transId });
                return res.ToList();
            }
        }

        public async Task<List<Transportation_Itineraries>> GetTransportation_ItinerariesAsync()
        {
            string query = $@"SELECT * FROM Transportation_Itineraries where isActive = 1";

            using (var connection = GetTickAndDashConnection())
            {
                var trans = await connection.QueryAsync<Transportation_Itineraries>(query);
                return trans.ToList();
            }
        }

        public async Task<int> Insert(Transportation_Itineraries transportation_Itineraries)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO {_itineraryTable}(Name, FromSiteId, Description, TowardSiteId, ItineraryTypeLookupId, IsActive, Price)
                                        OUTPUT INSERTED.Id
                                    VALUES(@Name, @FromSiteId, @Description, @TowardSiteId, @ItineraryTypeLookupId, @IsActive, @Price)";

                var id = await connection.ExecuteScalarAsync(query, transportation_Itineraries);
                return (int)id;
            }
        }

        public async Task<bool> UpdateAsync(Transportation_Itineraries transportation_Itineraries)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"UPDATE {_itineraryTable}
                                    SET Name = @Name
                                          ,FromSiteId = @FromSiteId
                                          ,Description = @Description
                                          ,TowardSiteId = @TowardSiteId
                                          ,ItineraryTypeLookupId = @ItineraryTypeLookupId
                                          ,IsActive = @IsActive
                                          ,Price = @Price
                                    WHERE Id = @Id
                              ";

                var result = await connection.ExecuteAsync(query, transportation_Itineraries);
                return result > 0;
            }
        }
    }
}
