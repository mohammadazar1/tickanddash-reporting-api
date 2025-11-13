using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class PointOfSalesDAL : BaseDAL, IPointOfSalesDAL
    {
        public PointOfSalesDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"DELETE FROM PointOfSales WHERE Id = @Id";
                var result = await connection.ExecuteAsync(query, new { Id = id });

                return result > 0;
            }
        }

        public async Task<List<PointOfSales>> GetAllPointOfsalesAsync(int siteId, string language)
        {
            string query = $@"SELECT {(language == "ar" ? "pos.id, pos.SiteId, pos.NameAr as 'Name', pos.AddressAr as 'Address', pos.DCToken" : 
                "pos.id, pos.SiteId, pos.NameEn as 'Name', pos.AddressEn as 'Address', pos.DCToken")} 
                                    , {(language != "ar" ? "sn.SiteId as 'Id', sn.Name " : "s.Id, s.Name")} 
                              FROM PointOfSales pos, Sites s {(language != "ar" ? " ,SitesNames sn" : "")} 
                              WHERE  s.Id = pos.SiteId
                                {(language != "ar" ? "and sn.SiteId = s.id and sn.Language = '" + language + "'" : "")} 
                            ";

            query = siteId > 0 ? string.Concat(query, "and pos.SiteId = @siteId") : query;

            using (var cn = GetTickAndDashConnection())
            {
                var result = await cn.QueryAsync<PointOfSales, Site, PointOfSales>(query, (pos, site) =>
                {
                    pos.Site = site;
                    return pos;
                }, new { siteId, language }
                );

                return result.ToList();
            }
        }

        public async Task<List<Site>> GetAllPOSSitesAsync(string language)
        {
            string query = $@"SELECT  distinct {(language != "ar" ? "sn.SiteId as 'Id', sn.Name " : "s.Id, s.Name")} 
                                 FROM PointOfSales pos, Sites s  {(language != "ar" ? " ,SitesNames sn" : "")}
                                 WHERE pos.SiteId = s.Id
                                      {(language != "ar" ? "and sn.SiteId = s.id and sn.Language = '" + language + "'" : "")} 
                            ";

            using (var cn = GetTickAndDashConnection())
            {
                var res = await cn.QueryAsync<Site>(query);

                return res.ToList();
            }
        }



        public async Task<bool> InsertAsync(PointOfSales pointOfSales)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO PointOfSales(NameAr, AddressAr, SiteId, NameEn, AddressEn)
                                    VALUES(@Name, @Address, @SiteId, @Name, @Address)";

                var reslult = await connection.ExecuteAsync(query, pointOfSales);

                return reslult > 0;
            }
        }

        public async Task<bool> UpdateAsync(PointOfSales pointOfSales)
        {
            using (var connectrion = GetTickAndDashConnection())
            {
                var query = $@"UPDATE PointOfSales
                                    SET Name = @Name
                                          ,Address = @Address
                                          ,SiteId = @SiteId
                                    WHERE Id = @Id
                             ";

                var result = await connectrion.ExecuteAsync(query, pointOfSales);

                return result > 0;

            }
        }

        public PointOfSales GetPOSByUsername(string username)
        {
            using (var cn = GetTickAndDashConnection())
            {
                return cn.Query<PointOfSales>(@"SELECT * FROM PointOfSales
                                                where Username = @username"
                                             , new { username }).FirstOrDefault();
                //return res.ToList();
            }
        }

        public Task<PointOfSales> GetPointOfSaleByUserIdAsync(int userId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PointOfSales> GetPointOfSaleByIdAsync(int id)
        {
            using (var cn = GetTickAndDashConnection())
            {
                return (await cn.QueryAsync<PointOfSales>(@"SELECT * FROM PointOfSales
                                                where userId = @id"
                                           , new { id })).FirstOrDefault();
            }

        }

    
    }
}
