namespace TickAndDashReportingTool.Controllers.V1
{
    public class GeneralResponse<T> where T : new()
    {
        public bool Success { get; set; }
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public T Data { get; set; } = new T();
    }

}