using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IMajorsMinorStationsDAL
    {
        Task<List<MajorsMinorStations>> GetMinorPickupStationsThatFollowsMainPickupStationAsync(int mainPickupStation, string language);
        Task<List<MajorsMinorStations>> GetMinorPickupStationsInSiteWithinSpecificTrans(int fromSiteId, int transId, string language);
        Task<MajorsMinorStations> GetMajorsMinorStationsByMinorStationId(int minorId);
        Task<bool> InsertAsync(MajorsMinorStations majorsMinorStations);
    }
}
