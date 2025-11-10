using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class RidersQueueDAL : BaseDAL, IRidersQueueDAL
    {
        public async Task<int> AddToRidersQueueAsync(RidersQueue ridersQueue)
        {
            string query = $@"insert into RidersQueue(RiderId, CreationDate, ReservationDate, SkipCount,  PickupStationId, RidersQStatusLookupId, CountOfSeats, IsInQueue)
                               OUTPUT INSERTED.Id
              VALUES (@RiderId, @CreationDate, @ReservationDate ,0,  @PickupStationId,  @RidersQStatusLookupId, @CountOfSeats, @IsInQueue)";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteScalarAsync<int>
                    (query, new
                    {
                        ridersQueue.RiderId,
                        ridersQueue.ReservationDate,
                        ridersQueue.CreationDate,
                        ridersQueue.PickupStationId,
                        ridersQueue.RidersQStatusLookupId,
                        ridersQueue.CountOfSeats,
                        ridersQueue.IsInQueue
                    });
            }
        }

        //public bool CancelAllExpiredReservation(int riderId)
        //{
        //    string query = $@"WITH UpdateCarsQueue AS (
        //                        SELECT  * from RidersQueue 
        //                     where RiderId = @riderId
        //                     and convert(date,CreationDate) = @creationDate
        //                        and ReservationDate < @nowDate
        //                        and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}
        //                    )
        //                    update UpdateCarsQueue 
        //                    set RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.PopedOut}, IsInQueue = 0 ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Execute(query, new
        //        {
        //            riderId,
        //            creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
        //            nowDate = DateTime.Now
        //        }) > 0;
        //    }
        //}
        public List<RidersQueue> GetAllRidersInQ(RidersQStatusLookupEnum status, int ps)
        {
            throw new NotImplementedException();
            // top 7 remove data limitation
            string query = $@"  SELECT rq.Id, rq.CreationDate, rq.ReservationDate, rq.RiderId, rq.SkipCount, rq.RidersQStatusLookupId, rq.CountOfSeats,                                     rq.PickupStationId, 
                                       u.Id, u.Name 
                                FROM  RidersQueue rq, Users u 
                                WHERE rq.RiderId = u.Id 
                                      and convert(date,rq.CreationDate) = @creationDate
                                      and rq.RidersQStatusLookupId = {(int)status}
                                      and rq.ReservationDate > '{DateTime.Now}'
                                      and rq.PickupStationId = @ps
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<RidersQueue, User, RidersQueue>(query, (rq, u) =>
                {
                    rq.User = u;
                    return rq;
                },
                    new { creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), ps },
                    splitOn: "Id"
                    ).ToList();
            }
        }

        //Count of seates and skips
        public int GetCountOfRidersInFrontOfMyTurn(DateTime reservationdatetime, int psId)
        {
            throw new NotImplementedException();
            string query = $@" select count(CountOfSeats) from RidersQueue
                                where ReservationDate <= @reservationdatetime     
                                and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}
                                and PickupStationId = @psId
                                and ReservationDate >= '{DateTime.Now}'
                                and IsInQueue = 1
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<int>(query, new { reservationdatetime, psId }).FirstOrDefault();
            }
        }

        public async Task<int> GetRaiderDailyCancellationCountAsync(int riderId)
        {
            string query = $@"  select count(id) from RidersQueue
                                where RiderId = @riderId
                                and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Canceled}
                                and convert(date,CreationDate) = @TDate
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { riderId, TDate = DateTime.Now.Date.ToString("yyyy-MM-dd") });

                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetRaiderCancellationCountAsync(int riderId)
        {
            string query = $@"  select count(id) from RidersQueue
                                where RiderId = @riderId
                                and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Canceled}
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { riderId });
                return result.FirstOrDefault();
            }

        }

        public async Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId)
        {
            string query = $@"  select count(id) from RidersQueue
                                where RiderId = @riderId
                                and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Canceled}
                                and convert(date,CreationDate) > @lastweekDate
                              ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { riderId, lastweekDate = DateTime.Now.Date.AddDays(-7).ToString("yyyy-MM-dd") });

                return result.FirstOrDefault();
            }
        }

        public async Task<RidersQueue> GetRidersInQAsync(int riderQId, RidersQStatusLookupEnum status)
        {
            throw new NotImplementedException();
            string query = $@"SELECT TOP 1 riderq.Id, riderq.CreationDate, riderq.RiderId, riderq.SkipCount,riderq.RidersQStatusLookupId,riderq.isInQueue,
                              riderq.CountOfSeats, riderq.ReservationDate, u.Id, u.FCMToken
                              from RidersQueue riderq, Users u                
                              where riderq.id = @riderQId
                              and riderq.RiderId = u.Id
                              and RidersQStatusLookupId = {(int)status}
                              order by riderq.id desc
                             ";
            //and riderq.ReservationDate < '{DateTime.Now.AddMinutes(5)}'
            //and riderq.IsInQueue = 1

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<RidersQueue, User, RidersQueue>(query, (riderq, u) =>
                {
                    riderq.User = u;
                    return riderq;

                }, new { riderQId/*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/ }, splitOn: "Id,Id");

                return res.FirstOrDefault();
            }
        }

        public async Task<RidersQueue> GetRidersInQAsync(int riderId)
        {
            string query = $@"SELECT TOP 1 riderq.Id, riderq.CreationDate, riderq.RiderId, 
                              riderq.SkipCount,riderq.RidersQStatusLookupId,riderq.isInQueue,
                              riderq.CountOfSeats, riderq.ReservationDate, u.Id, u.FCMToken
                              from RidersQueue riderq, Users u                
                              where riderq.RiderId = @riderId
                              AND IsInQueue = 1
                              AND riderq.RiderId = u.Id
                              order by riderq.id desc
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<RidersQueue, User, RidersQueue>(query, (riderq, u) =>
               {
                   riderq.User = u;
                   return riderq;

               }, new { riderId /*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/ }, splitOn: "Id,Id");

                return res.FirstOrDefault();
            }
        }
        public async Task<RidersQueue> GetActiveRidersInQWithStatusAsync(int userId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation, int notificationMinutesToExpire)
        {
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            DateTime nowDate = DateTime.Now;

            //string query = $@"SELECT TOP 1 * from RidersQueue                 
            //               where RiderId = @userId
            //                  and RidersQStatusLookupId = {(int)status}
            //                  and IsInQueue = 1
            //                  and DATEDIFF(hour, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < @timeLimitToCancellReservation
            //                  order by CreationDate asc
            //               ";

            string query = $@"SELECT TOP 1 * from RidersQueue                 
	                          where RiderId = @userId
                                 and 
                                (
                                    RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting} or
                                    (
                                        RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified }
                                        and DATEDIFF(SECOND, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < 
                                        @notificationMinutesToExpire * 60.0
                                    )
                                )
                              and IsInQueue = 1
                              and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') / 3600.0
                               < @timeLimitToCancellReservation
                              order by CreationDate asc
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.
                    QueryAsync<RidersQueue>(query, new { userId, status, timeLimitToCancellReservation, notificationMinutesToExpire/* creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), nowDate = DateTime.Now*/ });

                return result.FirstOrDefault();
            }
        }


        public bool IsRiderInQueueWithStatus(int riderId, int psId, RidersQStatusLookupEnum status)
        {
            string query = $@"  SELECT TOP 1  id from RidersQueue 
	                            where RiderId = @riderId
                                and PickupStationId = @psId
                                and RidersQStatusLookupId = {(int)status}
                                and IsInQueue = 1
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<int>(query, new
                {
                    riderId,
                    psId
                    /* nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")*//* DateTime.Now.Date.ToString("yyyy-MM-dd")*/
                }).FirstOrDefault() > 0;
            }
        }

        public int GetPickupStationsIdForRiderInQueueWithStatus(int riderId, RidersQStatusLookupEnum status)
        {
            string query = $@"  SELECT TOP 1  PickupStationId from RidersQueue 
	                            where RiderId = @riderId
                                and RidersQStatusLookupId = {(int)status}
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<int>(query,
                    new
                    {
                        riderId
                        //,nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    }).FirstOrDefault();
            }
        }

        //public bool UpdateRiderSkipCount(int riderId, int skipCount)
        //{
        //    string query = $@"WITH UpdateCarsQueue AS (
        //                        SELECT TOP 1  * from RidersQueue 
        //                     where RiderId = @riderId
        //                        and RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}
        //                        order by CreationDate asc
        //                    )
        //                    update UpdateCarsQueue 
        //                    set SkipCount = @skipCount ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Execute(query, new
        //        {
        //            riderId,
        //            skipCount
        //            /*creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), skipCount*/
        //        }) > 0;
        //    }
        //}


        public async Task<bool> UpdateRiderStatusInQueueAsync(int riderQId, RidersQStatusLookupEnum status)
        {
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


            DateTime nowDate = DateTime.Now;

            string query;
            if (status == RidersQStatusLookupEnum.Canceled)
            {
                query = $@"UPDATE RidersQueue
                              SET RidersQStatusLookupId = @status
                                  ,IsInQueue = 0
                              WHERE id = @riderQId
                            ";
            }
            else if (status == RidersQStatusLookupEnum.notified)
            {

                query = $@"   update RidersQueue
                                set RidersQStatusLookupId = {(int)status}, NotificationDate = '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}'
                                where Id = @riderQId
                            ";
            }
            else
            {
                query = $@"UPDATE RidersQueue
                              SET RidersQStatusLookupId = @status
                              WHERE id = @riderQId
                            ";
            }

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    riderQId,
                    status = (int)status
                }) > 0;
            }
        }

        public bool CancelRiderReservation(int riderId)
        {
            string query = $@"  WITH UpdateRidersQueue AS (
                                select TOP 1 * from RidersQueue
                                where RiderId = @riderId
                                RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}  
                                order by  ReservationDate desc, CreationDate desc
                                )
                                update UpdateRidersQueue
                                set RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Canceled} 
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new { riderId }) > 0;
            }
        }

        public bool UpdateRiderIsInQueueStatus(int riderId, bool isInQueue)
        {
            string query = $@"  WITH UpdateRidersQueue AS (
                                select TOP 1 * from RidersQueue
                                where RiderId = @riderId
                                order by  ReservationDate desc, CreationDate desc
                                )
                                update UpdateRidersQueue
                                set IsInQueue = @isInQueue
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new { riderId, isInQueue }) > 0;
            }
        }

        public bool DoesRiderHasAnActiveReservationInTheQueue(int queueReservationtimeLimit, int notificationMinutesToExpire)
        {
            var nowDate = DateTime.Now;
            string query = $@" SELECT TOP 1  id from RidersQueue 
                                where
                                RiderId = @riderId and
                                ReservationDate < {nowDate.AddMinutes(queueReservationtimeLimit)}         
                                and (RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}
                                or (RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.notified} and 
                                DATEDIFF(SECOND, {nowDate}, notificationDate) AS DateDiff) < @notificationMinutesToExpire * 60.0
                                )
                               and IsInQueue = 1
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<int>(query, new { notificationMinutesToExpire }).FirstOrDefault() > 0;
            }
        }

        public async Task<List<RidersQueue>> GetAllAcitveRidersInPickupStationQWithStatus(RidersQStatusLookupEnum status, List<int> psId, int timeLimitToCancellReservation)
        {
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DateTime nowDate = DateTime.Now;

            string query = $@"SELECT  * from RidersQueue                 
	                          where PickupStationId in @psId and 
                               RidersQStatusLookupId = {(int)status}
                              and IsInQueue = 1
                              and DATEDIFF(SECOND, ReservationDate,
                                  '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') /3600.0 < @timeLimitToCancellReservation
                              order by CreationDate asc
                           ";

            // top 7 remove data limitation
            //var nowDate = DateTime.Now;
            //string query = $@"  SELECT top (7) rq.Id, rq.CreationDate, rq.ReservationDate, rq.RiderId, rq.SkipCount, rq.RidersQStatusLookupId, rq.CountOfSeats,                    
            //                    rq.PickupStationId, rqLookup.Id, rqLookup.Name,
            //                    u.Id, u.Name 
            //                    FROM  RidersQueue rq, RidersQStatusLookup rqLookup, Users u 
            //                    WHERE rq.RiderId = u.Id 
            //                          and rq.RidersQStatusLookupId = rqLookup.id
            //                          AND RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.Waiting}
            //                          and rq.ReservationDate < '{DateTime.Now.AddMinutes(queueReservationtimeLimit)}'
            //                          and rq.PickupStationId = @psId
            //                          and rq.IsInQueue = 1
            //                        ";

            //and(RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.Waiting}
            //or RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.Accepted } or(RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified}
            //and
            //                          DATEDIFF(minute, notificationDate, '{nowDate}') < @notificationMinutesToExpire
            //                          ))

            using (var sqlConnection = GetTickAndDashConnection())
            {
                //return sqlConnection.Query<RidersQueue, RidersQStatusLookup, User, RidersQueue>(query, (rq, rqlookup, u) =>
                //{
                //    rq.RidersQStatusLookup = rqlookup;
                //    rq.User = u;
                //    return rq;
                //},
                //    new { psId , timeLimitToCancellReservation },
                //    splitOn: "Id,Id"
                //    ).ToList();
                var result = await sqlConnection.QueryAsync<RidersQueue>(query,
                    new { psId, timeLimitToCancellReservation }
                    );

                return result.ToList();

            }
        }

        public bool UpdateRiderStatusInQueueByRiderqId(int riderQId, RidersQStatusLookupEnum status)
        {
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //DateTime newDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DateTime newDate = DateTime.Now;

            string query = $@"   update RidersQueue
                                set RidersQStatusLookupId = {(int)status}, NotificationDate = '{newDate.ToString("yyyy-MM-dd HH:mm:ss")}'
                                where Id = @riderQId
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new { riderQId }) > 0;
            }
        }

        public async Task<List<RidersQueue>> GetActiveSeatViewRidersReservationInPickupStationAsync(List<int> pickups,
            int notificationMinutesToExpire, int timeLimitToCancellReservation, string language)
        {
            // top 7 remove data limitation
            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            DateTime nowDate = DateTime.Now;

            //string query = $@"  SELECT top (7) rq.Id, rq.CreationDate, rq.ReservationDate, rq.RiderId, rq.SkipCount, rq.RidersQStatusLookupId, rq.CountOfSeats,                    
            //                    rq.PickupStationId, rqLookup.Id, rqLookup.Name,
            //                    u.Id, u.Name 
            //                    ,ps.id, ps.Descriptions,
            //                    s.Id, {(language == "ar" ? "s." : "sn.")}Name  
            //                    FROM  RidersQueue rq, RidersQStatusLookup rqLookup, Users u, PickupStations ps, Sites s, SitesNames sn
            //                    WHERE rq.RiderId = u.Id and s.Id = sn.SiteId 
            //                        and rq.RidersQStatusLookupId = rqLookup.id
            //                        and ps.id = rq.PickupStationId
            //                        and ps.SiteId  = s.Id
            //                        and ( 
            //                            rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Accepted}
            //                            or rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Confirmed}
            //                            or rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Ticket}
            //                            or( RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified}
            //                        and
            //                             DATEDIFF(minute, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < @notificationMinutesToExpire
            //                        ) 
            //                        )
            //                        and  DATEDIFF(hour, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < @timeLimitToCancellReservation
            //                        and rq.PickupStationId in @pickups
            //                        and rq.IsInQueue = 1
            //                    order by rq.ReservationDate asc, rq.CreationDate asc
            //                ";

            string query = $@"  SELECT top (7) rq.Id, rq.CreationDate, rq.ReservationDate, rq.RiderId, rq.SkipCount, rq.RidersQStatusLookupId, rq.CountOfSeats,                    
                                rq.PickupStationId,rq.IsPresent, rqLookup.Id, rqLookup.Name,
                                u.Id, u.Name 
                                ,ps.id, ps.Descriptions, psTra.Name,
                                s.Id, s.Name  
                                FROM  RidersQueue rq, RidersQStatusLookup rqLookup, Users u, PickupStations ps, Sites s , PickupStationsTranslations psTra
                                WHERE rq.RiderId = u.Id
                                    and rq.RidersQStatusLookupId = rqLookup.id
                                    and ps.id = rq.PickupStationId
                                    and ps.SiteId  = s.Id
                                    and psTra.PickupStationId = ps.Id
                                    and ( 
                                        rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Accepted}
                                        or rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Confirmed}
                                        or rq.RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Ticket}
                                        or( RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified}
                                    and
                                         DATEDIFF(SECOND, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < @notificationMinutesToExpire * 60.0
                                    ) 
                                    )
                                    and  DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') / 3600.0 < @timeLimitToCancellReservation
                                    and rq.PickupStationId in @pickups
                                    and rq.IsInQueue = 1
                                    and psTra.Lang = @language
                                order by rq.ReservationDate asc, rq.CreationDate asc
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<RidersQueue, RidersQStatusLookup, User, PickupStations, Site, RidersQueue>(query,
                    (rq, rqlookup, u, pStation, s) =>
               {
                   rq.PickupStations = pStation;
                   rq.PickupStations.Sites = s;
                   rq.RidersQStatusLookup = rqlookup;
                   rq.User = u;
                   return rq;
               },

               new { notificationMinutesToExpire, timeLimitToCancellReservation, pickups = pickups.ToArray(), language },
                splitOn: "Id,Id,Id"
                );

                return result.ToList();
            }
        }

        public async Task<RidersQueue> GetRiderActiveReservationAsync(int riderId, int notificationMinutesToExpire,
            int timeLimitToCancellReservation)
        {
            //string query = $@"  SELECT TOP 1 riderq.Id, riderq.CreationDate, riderq.RiderId,                                                               riderq.SkipCount,riderq.RidersQStatusLookupId,riderq.isInQueue,
            //                    riderq.CountOfSeats, riderq.ReservationDate, u.Id, u.FCMToken
            //                    from RidersQueue riderq, Users u  
            //                 where RiderId = @riderId
            //                    and riderq.RiderId = u.Id
            //                    and ( RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Waiting}
            //                    or RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Accepted}
            //                    or RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.notified}
            //                    or RidersQStatusLookupId = {(int)RidersQStatusLookupEnum.Confirmed}
            //                    )";
            // top 7 remove data limitation

            //var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            //clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;
            //Thread.CurrentThread.CurrentCulture = clone;
            //Thread.CurrentThread.CurrentUICulture = clone;
            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            DateTime nowDate = DateTime.Now;

            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

            string query = $@" SELECT TOP 1 riderq.Id, riderq.CreationDate, riderq.ReservationDate,riderq.RiderId,  riderq.PickupStationId ,                                                                                           Riderq.SkipCount, riderq.RidersQStatusLookupId, riderq.isInQueue, riderq.SkipCount,
                                                       riderq.CountOfSeats, riderq.ReservationDate, 
                                                        u.Id, u.FCMToken,
                                                       riderqLookup.Id, riderqLookup.Name 
                                FROM RidersQueue riderq, RidersQStatusLookup riderqLookup , Users u  
	                            WHERE RiderId = @riderId
                                and riderq.RiderId = u.Id
                                and IsInQueue = 1
                                and riderqLookup.id = riderq.RidersQStatusLookupId
                                and 
                                (
                                    riderq.RidersQStatusLookupId != {(int)RidersQStatusLookupEnum.notified} or
                                    (
                                        RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified }
                                        and DATEDIFF(SECOND, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') <  @notificationMinutesToExpire * 60.0
                                    )
                                )
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') /3600.0 < @timeLimitToCancellReservation
                              ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<RidersQueue, User, RidersQStatusLookup, RidersQueue>(query, (rq, u, rqlookup) =>
               {
                   rq.User = u;
                   rq.RidersQStatusLookup = rqlookup;
                   return rq;
               },
                    new
                    {
                        riderId,
                        notificationMinutesToExpire,
                        timeLimitToCancellReservation
                        //,nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    },
                    splitOn: "Id,Id"
                    );

                return result.FirstOrDefault();
            }
        }

        public List<RidersQueue> GetAllByPickupStationId(int id)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = @"SELECT * FROM RidersQueue rq, Users u, RidersQStatusLookup rql, PickupStations ps, Transportation_Itineraries ti  
                            WHERE --PickupStationId = @Id AND
                                 rq.PickupStationId = ps.Id 
                                AND rq.RiderId = u.Id 
                                AND rq.RidersQStatusLookupId = rql.Id
                                AND ps.TransItineraryId = ti.Id
                                AND CAST(rq.CreationDate as date) = CAST(GETDATE() as date)
                            ORDER BY rq.Id DESC";
                return connection.Query<RidersQueue, User, RidersQStatusLookup, PickupStations, Transportation_Itineraries, RidersQueue>(sql,
                    (riderQueue, user, riderQLookup, pickupStations, itinerary) =>
                    {
                        riderQueue.User = user;
                        riderQueue.RidersQStatusLookup = riderQLookup;
                        pickupStations.Transportation_Itineraries = itinerary;
                        riderQueue.PickupStations = pickupStations;
                        return riderQueue;
                    },
                    new { Id = id },
                    splitOn: "Id, Id, Id, Id"
                    ).ToList();
            }
        }
        public int GetQLastRiderTurn(int pickupId)
        {
            string query = $@"SELECT max(Turn) from RidersQueue
                              WHERE  PickupStationId = @pickupId
                                      and IsInQueue = 1
                             ";

            using (var connection = GetTickAndDashConnection())
            {
                return connection.Query<int?>(query, new { pickupId }).FirstOrDefault() ?? 0;
            }
        }

        public async Task<bool> IsRiderInAnyQueueAsync(int riderId)
        {
            string query = $@"  SELECT TOP 1  id from RidersQueue 
	                            where RiderId = @riderId
                                and IsInQueue = 1
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int?>(query, new
                {
                    riderId,
                });

                return result.FirstOrDefault() > 0;
            };
        }

        public int GetRiderTurn(int psId, int riderId)
        {
            string query = $@"  SELECT TOP 1  Turn from RidersQueue 
	                            where RiderId = @riderId
                                and PickupStationId = @psId
                                and IsInQueue = 1
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<int>(query, new
                {
                    riderId,
                    psId
                }).FirstOrDefault();
            };
        }

        public async Task<RidersQueue> GetRiderInAnyQueueWithStatusAsync(int riderId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation)
        {
            //string query = $@"  SELECT TOP 1  id from RidersQueue 
            //                 where RiderId = @riderId
            //                    and RidersQStatusLookupId = {(int)status}
            //                    and IsInQueue = 1
            //               ";

            DateTime nowDate = DateTime.Now;
            string query = $@"  SELECT TOP 1 riderq.Id, riderq.PickupStationId
                                from RidersQueue riderq, Users u                
                                where 
                                riderq.RiderId = u.Id
                                and IsInQueue = 1
                                and u.Id = @riderId
                                and RidersQStatusLookupId = {(int)status}
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') /3600.0 < @timeLimitToCancellReservation
                                order by riderq.Id desc                                                           
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<RidersQueue>(query, new
                {
                    riderId,
                    timeLimitToCancellReservation
                })).FirstOrDefault();
            };
        }

        public async Task<int> GetCountOfRidersAfterMyTurnAsync(int riderId, int psId)
        {
            throw new NotImplementedException();
            string query = $@"  select count(id) from RidersQueue
                                where 
                                PickupStationId = @psId
                                AND  IsInQueue = 1
                                and turn > (
                                SELECT Turn FROM RidersQueue
                                WHERE RiderId = @riderId
                                AND  IsInQueue = 1
                                and PickupStationId = @psId
                                )
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int?>(query, new
                {
                    riderId,
                    psId
                });

                return res.FirstOrDefault() ?? 0;
            };
        }

        public async Task<int> GetCountOfReservationBeforeMeAsync(DateTime reservationTime, DateTime creationDate, List<int> psIds, int notificationMinutesToExpire,
            int timeLimitToCancellReservation, int riderQId)
        {

            //var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            //clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;
            //Thread.CurrentThread.CurrentCulture = clone;
            //Thread.CurrentThread.CurrentUICulture = clone;

            //System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;

            //DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            string nowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //string query = $@"SELECT count(id)
            //                  FROM RidersQueue
            //                  where (ReservationDate < @reservationTime or (ReservationDate = @reservationTime and CreationDate < @creationDate))
            //                  and IsInQueue = 1
            //                     and 
            //                    (
            //                        RidersQStatusLookupId != {(int)RidersQStatusLookupEnum.notified} or
            //                        (
            //                            RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified }
            //                            and DATEDIFF(minute, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') <           @notificationMinutesToExpire
            //                        )
            //                    )
            //                    and DATEDIFF(hour, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') < @timeLimitToCancellReservation
            //                     and PickupStationId = @psId

            //                    ";   

            string query = $@"SELECT count(id)
                              FROM RidersQueue
                              where (ReservationDate < @reservationTime or (ReservationDate = @reservationTime and CreationDate < @creationDate))
                              and IsInQueue = 1
                                 and 
                                (
                                    RidersQStatusLookupId != {(int)RidersQStatusLookupEnum.notified} or
                                    (
                                        RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.notified }
                                        and DATEDIFF(SECOND, notificationDate, '{nowDate}') <           @notificationMinutesToExpire * 60.0
                                    )
                                )
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate}') /3600.0 < @timeLimitToCancellReservation
                                and PickupStationId in @psIds
                                and id != @riderQId
                              ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new
                {
                    reservationTime = reservationTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    psIds = psIds.ToList(),
                    notificationMinutesToExpire,
                    creationDate = creationDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    timeLimitToCancellReservation,
                    riderQId
                });

                return result.FirstOrDefault();
            }
        }


        public async Task<bool> UpdateRiderSkipTimeAsync(int riderIQd, TimeSpan timeSpan, DateTime ReservationDate)
        {
            string query = $@"update RidersQueue SET  SkipCount+=1, SkipTime = @timeSpan  , ReservationDate= @ReservationDate, RidersQStatusLookupId = 
                              {(int)RidersQStatusLookupEnum.Waiting}
                             Where  id = @riderIQd";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query,
                    new
                    {
                        riderIQd,
                        timeSpan,
                        ReservationDate
                    }) > 0;
            }
        }


        public async Task<List<RidersQueue>> GetFirstNRidersInPickupStationQAsync(List<int> ps, int seatCounts, int queueReservationtimeLimit, int notificationMinutesToExpire,
            int timeLimitToCancellReservation)
        {
            //string query = $@"  SELECT top (7) rq.Id, rq.CreationDate, rq.ReservationDate, rq.RiderId, rq.SkipCount, rq.RidersQStatusLookupId, rq.CountOfSeats,    
            //                    rq.PickupStationId, rqLookup.Id, rqLookup.Name,
            //                    u.Id, u.Name 
            //                    FROM  RidersQueue rq, RidersQStatusLookup rqLookup, Users u 
            //                    WHERE rq.RiderId = u.Id 
            //                    and rq.RidersQStatusLookupId = rqLookup.id
            //                    AND RidersQStatusLookupId = { (int)RidersQStatusLookupEnum.Waiting}
            //                    and rq.ReservationDate < '{DateTime.Now.AddMinutes(queueReservationtimeLimit)}'
            //                    and rq.PickupStationId = @psId
            //                    and rq.IsInQueue = 1
            //                ";
            //var param = new DynamicParameters();
            //param.Add("@seatsCount", dbType: DbType.Int32, value: 7, direction: ParameterDirection.Input);
            //param.Add("@psId", dbType: DbType.Int32, value: ps, direction: ParameterDirection.Input);
            //param.Add("@reservationDate", dbType: DbType.DateTime2, value: queueReservationtimeLimit, direction: ParameterDirection.Input);

            string pickupStations = string.Join(",", ps);
            using (var sqlConnection = GetTickAndDashConnection())
            {
                //return sqlConnection.Query<RidersQueue>("GetFirstNRidersSeatsCountInPickupStationQ",
                //    new { seatsCount = 7, psId = ps, reservationDate = DateTime.Now.AddMinutes( queueReservationtimeLimit )}
                //    , commandType: CommandType.StoredProcedure).
                //    ToList();
                var result = await sqlConnection.
                    QueryAsync<RidersQueue, RidersQStatusLookup, User, PickupStations, RidersQueue>("GetFirstNRidersSeatsCountInPickupStationQ", (rq, rqlookup, u, pickup) =>
                 {
                     rq.PickupStations = pickup;
                     rq.RidersQStatusLookup = rqlookup;
                     rq.User = u;
                     return rq;
                 },
                  new
                  {
                      seatsCount = seatCounts,
                      psId = pickupStations,
                      reservationDate = DateTime.Now.AddMinutes(queueReservationtimeLimit),
                      timeLimitToCancellReservation
                  },
                  splitOn: "Id,Id,id",
                  commandType: CommandType.StoredProcedure
                  );

                return result.ToList();
            }
        }

        public async Task<List<RidersQueue>> GetRidersQOfCarTripAsync(int carQId)
        {
            //string query = @" SELECT TR.RiderQId as 'Id', TR.RiderId, U.Id, U.FCMToken from TripsRiders tr, CarsTrips ct, Drivers d, Users u 
            //                    WHERE tr.TripId = ct.Id and d.UserId = TR..RiderId and d.UserId = u.Id
            //                    AND ct.CarsQueueId = @carQId";
            string query = @"select TR.RiderQId as 'Id', TR.RiderId, rq.PickupStationId, rq.ReservationDate, rq.RidersQStatusLookupId, rq.IsPresent, U.Id, U.FCMToken, u.Language 
                             from   CarsTrips ct, TripsRiders tr, Users u, RidersQueue rq
                                where tr.RiderId = u.Id  
                                and ct.Id = tr.TripId 
                                and tr.riderQId = rq.id
                                and CarsQueueId = @carQId
                              order by rq.ReservationDate asc
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<RidersQueue, User, RidersQueue>(query, (rq, u) =>
               {
                   rq.User = u;
                   return rq;
               }, new
               {
                   carQId,
               }, splitOn: "Id");

                return result.ToList();
            }
        }

        public async Task<bool> UpdateRidersInQStatusAsync(List<int> ridersQIds, bool isInQueue)
        {
            string query = @"update RidersQueue 
                             set IsInQueue = 0
                             where id in @ridersQIds";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    ridersQIds,
                    isInQueue
                }) > 0;
            }
        }

        public async Task<RidersQueue> GetRidersInQWithStatusAsync(int riderQId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation)
        {
            //    System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("en-US", true);
            //    customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd h:mm tt";
            //    System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //    System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            //    DateTime nowDate = System.Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DateTime nowDate = DateTime.Now;


            string query = $@"SELECT TOP 1 riderq.Id, riderq.CreationDate, riderq.RiderId, riderq.SkipCount,riderq.RidersQStatusLookupId,riderq.isInQueue,riderq.PickupStationId,
                              riderq.CountOfSeats, riderq.ReservationDate, u.Id, u.FCMToken, u.Language
                              from RidersQueue riderq, Users u                
                              where riderq.id = @riderQId
                              and riderq.RiderId = u.Id
                              and IsInQueue = 1
                              and RidersQStatusLookupId = {(int)status}
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') / 3600.0 < @timeLimitToCancellReservation
                             ";
            //and riderq.ReservationDate < '{DateTime.Now.AddMinutes(5)}'
            //and riderq.IsInQueue = 1

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<RidersQueue, User, RidersQueue>(query, (riderq, u) =>
               {
                   riderq.User = u;
                   return riderq;
               }, new { riderQId, timeLimitToCancellReservation/*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/ }, splitOn: "Id,Id");

                return res.FirstOrDefault();

            }
        }

        public async Task<int> CreateManualTicket(RidersQueue ridersQueue)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO RidersQueue(RiderId, PickupStationId, 
                                RidersQStatusLookupId, CountOfSeats, ReservationDate, IsInQueue)
                                OUTPUT Inserted.ID
                               VALUES(@RiderId, @PickupStationId, @RidersQStatusLookupId, 
                               @CountOfSeats, @ReservationDate, @IsInQueue)";

                var result = await connection.ExecuteScalarAsync(query, ridersQueue);
                return (int)result;
            }
        }

        public async Task<List<RidersQueue>> GetRiderQNotifiedBookingWithNoUserResponseAsync(List<int> pickups, int notificationMinutesToExpire,
           int timeLimitToCancellReservation)
        {
            DateTime nowDate = DateTime.Now;
            string query = $@"SELECT rq.id, rq.PickupStationId, u.Id,u.Language, u.FCMToken 
                               FROM  RidersQueue Rq, Users u
                                Where
                                rq.RiderId = u.Id and
                                Rq.id in (
                                select id from RidersQueue
                                where IsInQueue = 1 and 
                                RidersQStatusLookupId =  {(int)RidersQStatusLookupEnum.notified} 
                                and DATEDIFF(SECOND, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}')>  @notificationMinutesToExpire * 60.0
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') /3600.0 < @timeLimitToCancellReservation 
                                and PickupStationId in @pickups
                                )
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<RidersQueue, User, RidersQueue>(query, (rq, u) =>
                {
                    rq.User = u;
                    return rq;
                }, new
                {
                    pickups,
                    notificationMinutesToExpire,
                    timeLimitToCancellReservation,
                }, splitOn: "id")).ToList();
            }
        }



        public async Task UpdateRiderQNotifiedBookingWithNoUserResponseToAccepted(List<int> pickups, int notificationMinutesToExpire,
            int timeLimitToCancellReservation)
        {
            DateTime nowDate = DateTime.Now;
            string query = $@"Update RidersQueue set RidersQStatusLookupId =  {(int)RidersQStatusLookupEnum.Accepted}
                                Where id in (
                                select id from RidersQueue
                                where IsInQueue = 1 and 
                                RidersQStatusLookupId =  {(int)RidersQStatusLookupEnum.notified} 
                                and DATEDIFF(SECOND, notificationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}')>  @notificationMinutesToExpire * 60.0
                                and DATEDIFF(SECOND, ReservationDate, '{nowDate.ToString("yyyy-MM-dd HH:mm:ss")}') /3600.0 < @timeLimitToCancellReservation 
                                and PickupStationId in @pickups
                                )
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.ExecuteAsync(query, new
                {
                    pickups,
                    notificationMinutesToExpire,
                    timeLimitToCancellReservation,
                });
            }
        }

        public async Task<bool> UpdateRiderPresenceStatusAsync(int riderQId)
        {
            string query = @"update RidersQueue 
                            set IsPresent = 1
                            where id = @riderQId";


            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    riderQId
                }) > 0;
            }


        }

        public async Task<bool> GetRiderPresenceStatusAsync(int riderQId)
        {
            string query = @"select IsPresent
                             from  RidersQueue 
                                where Id = @riderQId
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<bool>(query, new
                {
                    riderQId,
                }
                )).FirstOrDefault();
            }
        }
    }
}
