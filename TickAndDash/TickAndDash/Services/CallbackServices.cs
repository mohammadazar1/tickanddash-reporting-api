using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class CallbackServices : ICallbackServices
    {
        private readonly ICallbacksDAL _callbackDAL;

        public CallbackServices(ICallbacksDAL callbackDAL)
        {
            _callbackDAL = callbackDAL;
        }

        public async Task<bool> InsertCallbackAsync(Callback callback)
        {
            return await _callbackDAL.InsertCallbackAsync(callback);
        }
    }
}
