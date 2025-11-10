using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IItinerariesService
    {
        IList<Transportation_Itineraries> GetAll(Controllers.V1.GetAllItinerariesRequset getAllItinerariesRequset);
        Task<int> CreateAsync(CreateItineraryRequest createItinerariesRequest);
        Task<bool> UpdateAsync(UpdateItineraryRequest updateItineraryRequest);
    }
}
