namespace TickAndDashDAL.Enums
{


    public enum DatabaseSchemas
    {
        Dbo
    }

    public enum DatabaseTables
    {
        Riders
    }

    public enum ItineraryTypeLookupEnum
    {
        One_Way = 1,
        Two_Way = 2
    }

    public enum RolesEnum
    {
        Admin = 1,
        Rider = 2,
        Driver = 3,
        ManualTicket = 5,
        Supervisor = 6,
        Unknown = 300
    }

    public enum PickupStationsLookupEnum
    {
        Main = 1,
        Minor = 2
    }

    public enum CarsQStatusLookupEnum
    {
        InQueue = 1,
        Cancelled = 2,
        PoppedOut = 4,
        Passed = 5,
        offlineCancellation = 6
    }

    public enum RidersQStatusLookupEnum
    {
        Booked = 3,
        Confirmed = 4,
        Canceled = 5,
        PopedOut = 7,
        Accepted = 8,
        Waiting = 9,
        notified = 10,
        Ticket = 11
    }

    public enum SettingKeyEnum
    {
        SeatsCount,
        ReserveTimeLimit,
        DCancellBookingCount,
        WCancellBookingCount,
        NotificationMinutesToExpireReservation,
        TimeLimitToCancellReservation,
        TimeLimitToAccessDriverQAgian,
        PinCodesLimit
    }

    public enum MobileOSEnum
    {
        iOS,
        Android
    }

    public enum LanguageEnum
    {
        ar,
        en
    }

    public enum ComplaintEnum
    {
        Open,
        Solved,
    }

    public enum TransformationStatusEnum
    {
        NoBalance,
        Ok,
        Failed,
        Valdata4
    }

    public enum UserTransactionsTypesEnum   
    {
        TripBilling = 1,
        Transfer = 2,
        Subscription =3
    }

}
