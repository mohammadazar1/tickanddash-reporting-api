using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ICarsService
    {
        List<Car> GetAllCars();
        bool CreateCar(CreateCarRequest createCarRequest);
        bool UpdateCar(UpdateCarRequest putCarRequest);
    }
}
