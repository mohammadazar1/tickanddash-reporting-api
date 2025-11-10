using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface ICallbackServices
    {
        Task<bool> InsertCallbackAsync(Callback callback);
        //Task<bool> GetCallbackByIdAsync(Callback callback);

    }
}
