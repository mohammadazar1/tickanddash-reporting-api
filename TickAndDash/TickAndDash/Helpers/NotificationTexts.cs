namespace TickAndDash
{

    public static class NotificationTexts
    {
        public static class RidersQNotification
        {
            public static string AcceptDriverNotificationTitle_ar = "تأكيد الحجز";
            public static string AcceptDriverNotificationBody_ar = "تم قبول الحجز بنجاح";
            public static string AcceptDriverNotificationTitle_en = "Accept Reservation";
            public static string AcceptDriverNotificationBody_en = "Your reservation has been Accepted successfully";

            public static string PayingNotificationTitle_ar = "الأجرة";
            public static string PayingNotificationBody_ar = "تم دفع {0} شيكل بنجاح ";
            public static string PayingNotificationTitle_en = ""; //Billing
            public static string PayingNotificationBody_en = "Successfully paid ILS {0}";

            public static string SkipDriverNotificationTitle_ar = "تخطي";
            public static string SkipDriverNotificationBody_ar = "قام الراكب بتأحيل حجزه";
            public static string SkipDriverNotificationTitle_en = "Skiping";
            public static string SkipDriverNotificationBody_en = "Rider turn has been skipped";

            public static string AttendanceNotificationTitle_ar = "تأكيد الحضور";
            public static string AttendanceNotificationBody_ar = "قام الراكب بتأكيد حضوره";
            public static string AttendanceNotificationTitle_en = "Attendance confirmation";
            public static string AttendanceNotificationBody_en = "The passenger has confirmed attendance";

        }

        public static class DriversQNotification
        {
            public static string ConfirmNotificationMsgTitle_ar = "تأكيد الحجز";
            public static string ConfirmNotificationMsgTitle_en = "Reserve confirmation";
            public static string ConfirmNotificationMsgBody_ar = "يرجى تاكيد الحجز، سعر التذكرة {0} شيكل";
            public static string ConfirmNotificationMsgBody_en = "Please confirm your reservation, ticket cost is {0} ILS";


            public static string HappyMsgNotificationTitle_ar = "";
            public static string HappyMsgNotificationTitle_en = ""; //Goodbye
            public static string HappyMsgNotificationBody_ar = "نتمنى لك رحلة ممتعة وآمنة";
            public static string HappyMsgNotificationBody_en = "We wish you a happy and safe journey";


            public static string HappyMsgNotificationForMinorStationBody_ar = "سيصل السائق إلى المحطة خلال {0} دقائق ، يرجى التواجد هناك";
            public static string HappyMsgNotificationForMinorStationBody_en = "The Driver will be at the station within {0} minutes, please be there";
        }

        public static class WalletNotification
        {
            public static string RefillNotificationMsgTitle_ar = "طلب تعبئة رصيد";
            public static string RefillNotificationMsgTitle_en = "request for refilling balance";

            public static string RefillNotificationMsgBody_ar = "طلب منك {0} تعبئة رصيد بقيمة {1} شيكل من الرصيد الخاص بك";
            public static string RefillNotificationMsgBody_en = "{0} request to recharge his balance with a value of {1} ILS which will be deducted from your balance";

            public static string RefillNotificationSuccessResponseMsgTitle_ar = " تعبئة رصيد";
            public static string RefillNotificationSuccessResponseMsgTitle_en = "Balance Refilling";

            public static string RefillNotificationSuccessResponseMsgBody_ar = "تم تعبئة {0} شيكل بنجاح";
            public static string RefillNotificationSuccessResponseMsgBody_en = "Refilling balance successfully with {0} ILS";

            public static string RefillNotificationFailedResponseMsgBody_ar = " لم يوافق السائق على عملية تحويل الرصيد";
            public static string RefillNotificationFailedResponseMsgBody_en = "Driver did not accept balance refilling request";

            public static string RefillNotificationFailureResponseMsgBody_ar = "عذرًا، لم تكتمل عملية شحن الرصيد بنجاح";
            public static string RefillNotificationFailureResponseMsgBody_en = "Sorry! the refilling balance process did not complete successfully";
        }

    }

}
