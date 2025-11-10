namespace TickAndDash.Controllers.V1.Responses
{
    //public class GetActiveSitesResponse
    //{
    //    public List<SitesToGo> SitesToGo { get; set; } = new List<SitesToGo>();
    //}

    //public class SitesToGo
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}


    public class GetAllSitesCanVisitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GetAllComplaintsType
    {
        public int ComplaintTypesId { get; set; }
        public string Type { get; set; }
    }

    public class GetAllComplaintsSubType
    {
        public int ComplaintsSubTypeId { get; set; }
        public string SubType { get; set; }
    }


}
