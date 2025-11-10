using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class PickupStationsDAL : BaseDAL, IPickupStationsDAL
    {
        public async Task<List<PickupStations>> GetAllActiveMainPickupStationsAsync()
        {
            string query = $@"select * from PickupStations
                              where IsActive = 1
                              and PickupTypeId = { (int)PickupStationsLookupEnum.Main }
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations>(query);

                return result.ToList();
            }
        }

        public async Task<List<PickupStations>> GetAllAsync()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"SELECT *
                                  FROM 
                                        PickupStations ps, 
                                        Transportation_Itineraries ti, 
                                        PickupStationsLookup psl, 
                                        Sites s,
                                        PickupStationsTranslations pstar,
                                        PickupStationsTranslations psten
                                  WHERE ps.TransItineraryId = ti.Id AND ps.PickupTypeId = psl.Id AND ps.SiteId = s.Id
                                        AND pstar.PickupStationId = ps.Id AND pstar.Lang LIKE 'ar'
                                        AND psten.PickupStationId = ps.Id AND psten.Lang LIKE 'en'";

                var result = await connection.QueryAsync<PickupStations, Transportation_Itineraries, PickupStationsLookup, Site, PickupStationsTranslations, PickupStationsTranslations, PickupStations>(query,
                    (pickup, trans, pickupLook, site, ar, en) =>
                    {
                        pickup.PickupStationsLookup = pickupLook;
                        pickup.Transportation_Itineraries = trans;
                        pickup.Sites = site;
                        pickup.NameAr = ar.Name;
                        pickup.NameEn = en.Name;

                        return pickup;
                    });

                return result.ToList();
            }
        }

        public async Task<List<PickupStations>> GetAllPickUpStationsByCarIdAsync(int carId, string language)
        {

            // changed to only main 
            //string query = $@"
            //            select ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, 
            //            pslook.Id, pslook.Type,
            //            {(language != "ar" ? "sn.SiteId as 'id', sn.Name," : " s.id, s.Name,")}
            //            trans.Id, trans.Name
            //            from PickupStations ps,  PickupStationsLookup pslook, Sites s,  {(language != "ar" ? " SitesNames sn," : "")}
            //            Transportation_Itineraries trans, Cars c
            //            where  pslook.Id = ps.PickupTypeId
            //            and s.Id = ps.SiteId
            //            and trans.Id = ps.TransItineraryId
            //            and s.Id = ps.SiteId
            //            {(language != "ar" ? "and sn.SiteId = s.id and sn.Language = '" + language + "'" : "")} 
            //            and c.ItineraryId = trans.Id
            //            and ps.IsActive = 1
            //            and trans.IsActive =1
            //            and s.IsActive = 1 
            //            and c.Id = @carId
            //            and pslook.id = {(int)PickupStationsLookupEnum.Main}
            //            ";

            string query = $@"
                        select ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, pst.Name,
                        pslook.Id, pslook.Type,
                        s.id, s.Name,
                        trans.Id, trans.Name
                        from PickupStations ps,  PickupStationsLookup pslook, Sites s,
                        Transportation_Itineraries trans, Cars c,
                        PickupStationsTranslations pst
                        where  pslook.Id = ps.PickupTypeId
                        and pst.PickupStationId = ps.Id
                        and s.Id = ps.SiteId
                        and trans.Id = ps.TransItineraryId
                        and s.Id = ps.SiteId
                        and c.ItineraryId = trans.Id
                        and ps.IsActive = 1
                        and trans.IsActive =1
                        and s.IsActive = 1 
                        and c.Id = @carId
                        and pslook.id = {(int)PickupStationsLookupEnum.Main}
                        and pst.Lang = @language
                        ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var pickupStations = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup,
                    Site, Transportation_Itineraries, PickupStations>(query, (ps, pslook, sites, itern) =>
                {
                    ps.PickupStationsLookup = pslook;
                    ps.Sites = sites;
                    ps.Transportation_Itineraries = itern;
                    return ps;
                }, new { carId, language }, splitOn: "Id, Id, Id");
                return pickupStations.ToList();
            }
        }

        public async Task<PickupStations> GetByIdAsync(int destinationMainPickup)
        {
            string query = $@"SELECT * FROM PickupStations
                              WHERE IsActive = 1
                              AND Id = { destinationMainPickup }";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations>(query);
                return result.FirstOrDefault();
            }
        }

        public async Task<List<PickupStations>> GetMainItineraryId(int id)
        {
            string query = $@"SELECT * FROM PickupStations
                              WHERE IsActive = 1 
                                AND TransItineraryId = @id
                                AND PickupTypeId = { (int)PickupStationsLookupEnum.Main }";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations>(query, new { id });

                return result.ToList();
            }
        }

        public async Task<int> GetMinorPickupStationMainStaionId(int psid)
        {
            string query = @"SELECT MainPickupStationId FROM MajorsMinorStations
                            WHERE MinorPickupStationId = @psid";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { psid });
            }
        }

        public async Task<PickupStations> GetPickupStationByIdAsync(int psId, string language)
        {
            //string query = $@"
            //                select ps.id, ps.TransItineraryId, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, ps.PickupTypeId, ps.siteId
            //                ,pslook.Id, pslook.Type, pslook.Description
            //                , itir.Id, itir.Name, itir.FromSiteId, itir.Description, itir.TowardSiteId, itir.ItineraryTypeLookupId
            //                ,ititlook.Id, ititlook.Description, ititlook.Name,
            //                {(language != "ar" ? "sn.SiteId as 'id', sn.Name" : " s.id, s.Name")}
            //                from PickupStations ps, PickupStationsLookup pslook, Transportation_Itineraries itir, ItineraryTypeLookup ititlook, Sites s {(language != "ar" ? ", SitesNames sn" : "")}
            //                where ps.TransItineraryId = itir.Id
            //                and itir.ItineraryTypeLookupId = ititlook.Id
            //                and ps.PickupTypeId = pslook.Id
            //                and s.id = ps.SiteId
            //                {(language != "ar" ? "and sn.SiteId = s.Id and sn.Language ='" + language + "'" : "")} 
            //                and ps.Id = @psId
            //                 ";

            string query = $@"
                            select ps.id, ps.TransItineraryId, ps.latitude, ps.longitude, ps.Radius,  pst.Name,
                                   ps.Descriptions, ps.PickupTypeId, ps.siteId,
                             pslook.Id, pslook.Type, pslook.Description,
                             itir.Id, itir.Name, itir.FromSiteId, itir.Description, itir.TowardSiteId, itir.ItineraryTypeLookupId,
                             ititlook.Id, ititlook.Description, ititlook.Name,
                             s.id, s.Name
                            from PickupStations ps, PickupStationsLookup pslook, Transportation_Itineraries itir,
                                 ItineraryTypeLookup ititlook, Sites s,
                                 PickupStationsTranslations pst
                            where ps.TransItineraryId = itir.Id
                            and itir.ItineraryTypeLookupId = ititlook.Id
                            and ps.PickupTypeId = pslook.Id
                            and pst.PickupStationId = ps.Id
                            and s.id = ps.SiteId
                            and ps.Id = @psId
                            and pst.Lang = @language
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup, Transportation_Itineraries, ItineraryTypeLookup, Site, PickupStations>
                    (query, (ps, pslook, itir, itirlook, sites) =>
                    {
                        itir.ItineraryTypeLookup = itirlook;
                        ps.PickupStationsLookup = pslook;
                        ps.Transportation_Itineraries = itir;
                        ps.Sites = sites;
                        return ps;
                    }, new { psId, language });

                return result.FirstOrDefault();
            }
        }

        public async Task<List<PickupStations>> GetPickupStationsEndPointsPickupStationsAsync(int psSiteId, int transId, string language)
        {
            string query = $@"
                            select  ps.id, ps.TransItineraryId, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, ps.PickupTypeId, ps.siteId,pstr.Name,
                            pslook.Id, pslook.Type, pslook.Description,
                            s.Id, s.Name,
                            trans.Id, trans.Name
                            from PickupStations ps, PickupStationsLookup pslook, Sites s, Transportation_Itineraries trans
                            , PickupStationsTranslations pstr
                            where 
                            ps.PickupTypeId = pslook.Id 
                            and pstr.PickupStationId = ps.Id
                            and trans.Id = ps.TransItineraryId
                            and s.Id = ps.SiteId 
                            and TransItineraryId =@transId
                            and ps.SiteId != @psSiteId
                            and pslook.id = {(int)PickupStationsLookupEnum.Main}
                            and pstr.Lang = @language
                            "
                            ;
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup, Site, Transportation_Itineraries, PickupStations>(query, (ps, pslook, s, itir) =>
               {
                   ps.Sites = s;
                   ps.PickupStationsLookup = pslook;
                   ps.Transportation_Itineraries = itir;
                   return ps;
               }, new { psSiteId, transId,language }
               , splitOn: "Id,Id,Id"
               );
                return result.ToList();
            }
        }

        public async Task<List<PickupStations>> GetSitePickupStationsByIdAndFromSiteAsync(int fromSite, int transId, string language)
        {
            //string query = $@"
            //                SELECT ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, 
            //                pslook.Id, pslook.Type,
            //                {(language != "ar" ? "sn.SiteId as 'Id', sn.Name " : "s.Id, s.Name")} 
            //                ,trans.Id, trans.Name
            //                FROM PickupStations ps,  PickupStationsLookup pslook, Sites s, Transportation_Itineraries trans
            //                    {(language != "ar" ? " ,SitesNames sn" : "")} 
            //                WHERE  pslook.Id = ps.PickupTypeId
            //                and s.Id = ps.SiteId
            //                and trans.Id = ps.TransItineraryId
            //                {(language != "ar" ? "and sn.SiteId = s.id and sn.Language = '" + language + "'" : "")} 
            //                and ps.SiteId  = @fromSite
            //                and ps.TransItineraryId = @transId
            //                and ps.IsActive = 1
            //                and trans.IsActive =1
            //                and s.IsActive = 1 
            //                and ps.PickupTypeId = 1
            //                ";

            string query = $@"
                            SELECT ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, psTra.Name,
                            pslook.Id, pslook.Type,
                            trans.Id, trans.Name,
                            s.id, s.Name
                            FROM PickupStations ps,  PickupStationsLookup pslook, Sites s, Transportation_Itineraries trans,
                            PickupStationsTranslations psTra
                            WHERE pslook.Id = ps.PickupTypeId
                            and s.Id = ps.SiteId
                            and trans.Id = ps.TransItineraryId
                            and psTra.PickupStationId = ps.Id
                            and ps.SiteId  = @fromSite
                            and ps.TransItineraryId = @transId
                            and ps.IsActive = 1
                            and trans.IsActive =1
                            and s.IsActive = 1
                            and ps.PickupTypeId = 1
                            and Lang = @language
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var pickupStations = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup, Site, Transportation_Itineraries, PickupStations>(query, (ps, pslook, sites, itern) =>
                {
                    ps.PickupStationsLookup = pslook;
                    ps.Sites = sites;
                    ps.Transportation_Itineraries = itern;
                    return ps;
                }, new { fromSite, transId, language }, splitOn: "Id, Id, Id");

                return pickupStations.ToList();
            }
        }

        public async Task<List<PickupStations>> GetSitePickupStationsForRangeOfSitesAsync(int[] siteId)
        {
            string query = $@"select ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, 
                            pslook.Id, pslook.Type
                            ,s.Id, s.Name, 
                            trans.Id, trans.Name
                             from PickupStations ps,  PickupStationsLookup pslook, Sites s, Transportation_Itineraries trans
                            where  pslook.Id = ps.PickupTypeId
                            and s.Id = ps.SiteId
                            and trans.Id = ps.TransItineraryId
                            and ps.SiteId  in @siteId
                            and ps.IsActive = 1
                            and trans.IsActive =1
                            and s.IsActive = 1
                            ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup, Site, Transportation_Itineraries, PickupStations>(query, (ps, pslook, sites, itern) =>
                {
                    ps.PickupStationsLookup = pslook;
                    ps.Sites = sites;
                    ps.Transportation_Itineraries = itern;
                    return ps;
                }, new { siteId }, splitOn: "Id, Id, Id");
                return result.ToList();
            }
        }

        public async Task<List<PickupStations>> GetUniqueMinorPickupStations(int carId, string language)
        {
            string query = $@"SELECT ps.id, ps.latitude, ps.longitude, ps.Radius, ps.Descriptions, pst.Name,
                            pslook.Id, pslook.Type,
                            s.id, s.Name,
                            trans.Id, trans.Name
                            from PickupStations ps,  PickupStationsLookup pslook, Sites s,
                            Transportation_Itineraries trans, Cars c,
                            PickupStationsTranslations pst
                            where  pslook.Id = ps.PickupTypeId
                            and pst.PickupStationId = ps.Id
                            and s.Id = ps.SiteId
                            and trans.Id = ps.TransItineraryId
                            and s.Id = ps.SiteId
                            and c.ItineraryId = trans.Id
                            and ps.IsActive = 1
                            and trans.IsActive =1
                            and s.IsActive = 1 
                            and c.Id = @carId
                            and pslook.id = {(int)PickupStationsLookupEnum.Minor}
                            and pst.Lang = @language
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var pickupStations = await sqlConnection.QueryAsync<PickupStations, PickupStationsLookup,
                    Site, Transportation_Itineraries, PickupStations>(query, (ps, pslook, sites, itern) =>
                    {
                        ps.PickupStationsLookup = pslook;
                        ps.Sites = sites;
                        ps.Transportation_Itineraries = itern;
                        return ps;
                    }, new { carId, language }, splitOn: "Id, Id, Id");
                return pickupStations.ToList();
            }
        }

        public async Task<int> InsertAsync(PickupStations pickupStations)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO PickupStations(TransItineraryId, IsActive, Latitude, Longitude, Radius, Descriptions, PickupTypeId, SiteId)
                                    OUTPUT INSERTED.Id
                                    VALUES(@TransItineraryId, @IsActive, @Latitude, @Longitude, @Radius, @Descriptions, @PickupTypeId, @SiteId)";

                var id = await connection.ExecuteScalarAsync<int>(query, pickupStations);

                return (int)id;
            }
        }

        public async Task<bool> InsertTranslationAsync(int pickupId, string nameAr, string nameEn)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO PickupStationsTranslations (PickupStationId, Lang, Descriptions, Name)
                                    VALUES ({pickupId}, 'ar', N'', N'{nameAr}'), ({pickupId}, 'en', N'', N'{nameEn}')";

                var result = await connection.ExecuteAsync(query);

                return result > 0;
            }
        }

        public async Task<bool> IsMajorPickupStationAsync(int psId)
        {
            string query = $@"SELECT id FROM PickupStations 
                             WHERE id = @psId 
                             and PickupTypeId = {(int)PickupStationsLookupEnum.Main}";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int>(query, new { psId });
                return res.FirstOrDefault() > 0;
            }
        }

        public async Task<bool> IsPickupstationActiveAndVaildForTheDriverAsync(int carId, int pickupId)
        {
            string query = $@"select c.Id from PickupStations ps, Cars c, Transportation_Itineraries itr
                            where c.ItineraryId = itr.Id and ps.TransItineraryId = itr.Id
                            and ps.Id = @pickupId
                            and c.Id = @carId
                            and c.IsActive = 1
                            and ps.IsActive = 1
                            and itr.IsActive = 1
                            and ps.PickupTypeId = { (int)PickupStationsLookupEnum.Main}
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int>(query, new { pickupId, carId });
                return res.FirstOrDefault() > 0;
            }
        }

        public async Task<bool> IsPickupStationValidAndActiveAsync(int psId)
        {
            string query = $@"select id from PickupStations
                            where IsActive = 1
                            and id = @psId ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { psId });
                return result.FirstOrDefault() > 0;
            }
        }

        public async Task<bool> UpdateAsync(PickupStations pickupStations)
        {
            using (var connection = GetTickAndDashConnection())
            {
                string query = $@"UPDATE PickupStations 
                                    SET TransItineraryId = @TransItineraryId
                                          ,IsActive = @IsActive
                                          ,latitude = @latitude
                                          ,longitude =@longitude
                                          ,Radius = @Radius
                                    WHERE Id = @Id
                                ";

                var result = await connection.ExecuteAsync(query, pickupStations);
                return result > 0;
            }
        }

        public async Task<bool> UpdatePickupTranslation(int id, string nameAr, string nameEn)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"UPDATE PickupStationsTranslations 
                                    SET Name = N'{nameAr}'
                                    WHERE PickupStationId = {id} AND Lang LIKE 'ar';

                                UPDATE PickupStationsTranslations 
                                    SET Name = N'{nameEn}'
                                    WHERE PickupStationId = {id} AND Lang LIKE 'en';";

                var result = await connection.ExecuteAsync(query);

                return result > 0;
            }
        }
    }
}
