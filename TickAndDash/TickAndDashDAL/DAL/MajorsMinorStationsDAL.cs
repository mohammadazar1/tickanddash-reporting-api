using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class MajorsMinorStationsDAL : BaseDAL, IMajorsMinorStationsDAL
    {
        public MajorsMinorStationsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<MajorsMinorStations> GetMajorsMinorStationsByMinorStationId(int minorId)
        {
            string query = @"SELECT *
                            FROM MajorsMinorStations
                            WHERE MinorPickupStationId = @minorId";

            using (var connection = GetTickAndDashConnection())
            {
                var result = await connection.QueryAsync<MajorsMinorStations>(query, new { minorId });
                return result.FirstOrDefault();
            }
        }
        public async Task<List<MajorsMinorStations>> GetMinorPickupStationsInSiteWithinSpecificTrans(int fromSiteId, int transId, string language)
        {
            string query = $@"select mp.id, mp.MinorPickupStationId, mp.FromSiteId,
                                p.Id, p.latitude, p.longitude, p.Radius, p.Descriptions, p.TransItineraryId,  psTra.Name,
                                s.Id, s.Name, 
                                trans.Id, trans.Name
                                from MajorsMinorStations mp ,Sites s, PickupStations p, Transportation_Itineraries trans,
                                PickupStationsTranslations psTra
                                where s.Id = p.SiteId
                                and mp.MinorPickupStationId = p.Id
                                and psTra.PickupStationId = p.Id
                                and trans.Id = p.TransItineraryId
                                and mp.FromSiteId = @fromSiteId    
                                and mp.TransItineraryId = @transId
                                and psTra.Lang = @language
                          ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<MajorsMinorStations, PickupStations, Site,
                    Transportation_Itineraries, MajorsMinorStations>(query, (mp, p, s, tr) =>
                    {
                        p.Sites = s;
                        p.Transportation_Itineraries = tr;
                        mp.MinorPickupStations = p;
                        return mp;
                    },
                new { fromSiteId, transId, language },
                splitOn: "Id,Id,Id"
                );
                return result.ToList();
            }
        }
        public async Task<List<MajorsMinorStations>> GetMinorPickupStationsThatFollowsMainPickupStationAsync(int mainPickupStationId, string language)
        {
            string query = @"SELECT mp.id, mp.MinorPickupStationId, mp.MainPickupStationId,
                                        p.Id, p.latitude, p.longitude, p.Radius, p.Descriptions, p.TransItineraryId,
                                         s.Id, s.Name,
                                         trans.Id, trans.Name
                                FROM MajorsMinorStations mp, PickupStations p , Sites s , Transportation_Itineraries trans
                                WHERE mp.MinorPickupStationId = p.Id and s.Id = p.SiteId
                                and p.TransItineraryId = trans.Id
                                and mp.MainPickupStationId = @mainPickupStationId
            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<MajorsMinorStations, PickupStations, Site,
                    Transportation_Itineraries, MajorsMinorStations>(query, (mp, p, s, tr) =>
                {
                    p.Sites = s;
                    p.Transportation_Itineraries = tr;
                    mp.MinorPickupStations = p;
                    return mp;
                },
                new { mainPickupStationId },
                splitOn: "Id,Id,Id"
                );

                return result.ToList();
            }
        }

        public async Task<bool> InsertAsync(MajorsMinorStations majorsMinorStations)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO MajorsMinorStations (FromSiteId, MinorPickupStationId, TransItineraryId, MainPickupStationId)
                                    VALUES(@FromSiteId, @MinorPickupStationId, @TransItineraryId, @MainPickupStationId)";

                var result = await connection.ExecuteAsync(query, majorsMinorStations);

                return result > 0;
            }
        }
    }
}
