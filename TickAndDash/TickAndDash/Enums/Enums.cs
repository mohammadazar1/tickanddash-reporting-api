namespace TickAndDash.Enums
{
    public enum Generalcodes
    {
        Ok, // success
        Internal, //Internal error
        Auth_1,// token is not valid
        Auth_2, // rider session is expired due to opening another one
        FCM_1, //
        Gen_1,
        Sub_1,
        Sub_2,
        Sub_3,
        Failed
    }

    public enum ValidationCodes
    {
        MobVal_1, // Mobile Number is not valid
        MobVal_2, // Request is not comming from Mobile device
        Pin_1, // wrong pin code
        Pin_2, // pincode Expired
        blok_1, //user is blocked from using the service
        LogOut_1,// user is not logged in;
        LatLng_1,// wrorng latitude or longitude value,
        Data_1, // No mathing data
        Pass_1, // incorrect password
        Itir_1, // Access denied
        PickUp_1, // pickup not active for this driver
        PickUp_2, // has an active reservation today
        Data_2, // sql query did not affect any row
        Date_1, // datetime is not valid 
        Param_1, // BadRequest by missing paramter request or passing unvalid value.
        Cancel_1,
        Active_1, // Not Active
        book_1,
    }

    public enum UnExpectedErrors
    {
        Pin_3, // failure when sending the pincode
        sql_1 // database failure
    }

    public enum UserCodes
    {
        Usr_1, // user already Exist
        Usr_2,  // User not found
    }

    public enum LocationCodes
    {
        loca_1, // Unable to find city location based on coordinates
        loca_2 // Unable to map the city to any cities in our end 
    }

    public enum QueusCode
    {
        car_1,  // Car already in the queue
        car_2,  // Car is not in the queue
        car_3,  // it is not your turn
        car_4,  // it is your turn
        rid_1,  // Not Accepted 
    }

    public enum ClaimsEnum
    {
        MobileNumber,
        Pincode,
        UserId,
        UserAgnet,
        IP,
        LicenseNumber,
        CarId,
        FCM
    }

    public enum WalletError
    {
        Wallet1
    }

    public enum SubStatus {
        Sub,
        UnsubActive,
        SubNoBalance,
        Unsub,
        SubRestored
    }

}
