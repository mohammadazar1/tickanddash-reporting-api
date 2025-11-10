using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using Dapper;
using System.Linq;

namespace TickAndDashDAL.DAL
{
    public class UserTransactionDAL : BaseDAL, IUserTransactionDAL
    {
        public async Task<IList<UserTransaction>> GetAllByFinancialsRequestAsync(UserTransactionFilter financialsRequest)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"SELECT SUM(ut.Amount) Amount, CONVERT(Date, ut.CreationDate) Date, 
                                    u.Name, c.RegistrationPlate, d.MobileNumber, d.LicenseNumber 
	                            FROM UserTransactions ut, Cars c, Drivers d, Users u
	                            WHERE ut.ToUserId = d.UserId AND c.Id = d.CarId AND u.Id = d.UserId
                                    {(financialsRequest.CarId == 0 ? "" : " AND d.CarId = @CarId")}
                                    AND CONVERT(Date, ut.CreationDate) >= @FromDate
                                    AND CONVERT(Date, ut.CreationDate) <= @ToDate
									AND ut.Type = 'TripBilling'
	                            GROUP BY CONVERT(Date, ut.CreationDate), u.Name, 
                                    c.RegistrationPlate, d.MobileNumber, d.LicenseNumber ";

                var result = await connection.QueryAsync<UserTransaction>(query, financialsRequest);
                return result.ToList();
            }
        }

        public async Task<IList<TotalFinancialBalance>> GetDriversFinancialsBalance()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"
                            SELECT  
                                DriverTransferedMoney, 
                                DriverTripMoney, 
                                PaymentsToDriver, 
                                LicenseNumber, 
                                Name,
                                CASE WHEN UserId IS NULL THEN DriverId ELSE UserId END UserId
                                FROM 
	                                (SELECT 
		                                SUM(CASE WHEN ut.UserTransactionTypeId = 2 AND ut.FromUserId = d.UserId THEN ut.Amount ELSE 0 END) DriverTransferedMoney, 
		                                SUM(CASE WHEN ut.UserTransactionTypeId = 1 AND ut.ToUserId = d.UserId THEN ut.Amount ELSE 0 END) DriverTripMoney,
		                                d.UserId, 
		                                d.LicenseNumber, 
		                                u.Name
	                                FROM UserTransactions ut
		                                INNER JOIN Drivers d
		                                ON ut.FromUserId = d.UserId OR ut.ToUserId = d.UserId
		                                INNER JOIN Users u
		                                ON u.Id = d.UserId
		                                GROUP BY d.UserId, d.LicenseNumber, u.Name
                                    ) trans
	                            FULL OUTER JOIN 
                                    (SELECT ptd.DriverId, SUM(ptd.paymentAmount) PaymentsToDriver
					                    FROM PaymentsToDrivers ptd
					                    GROUP BY ptd.DriverId
                                    ) pay
	                            ON trans.UserId = pay.DriverId";

                var data = await connection.QueryAsync<TotalFinancialBalance>(query);
                return data.ToList();
            }
        }
    }
}
