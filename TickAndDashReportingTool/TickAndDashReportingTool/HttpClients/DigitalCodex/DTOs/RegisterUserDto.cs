namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class RegisterUserDto
    {
        public bool CanUseBalance { get; internal set; } = false;
        public int CountryId { get; internal set; } = 1;
        public string Email { get; internal set; }
        //public int WorkflowId { get; internal set; } = 3;
        public string Location { get; internal set; }
        public string Name { get; internal set; }
        public string Password { get; internal set; }
        public string Role { get; internal set; } = "PoS";
        public string UserName { get; internal set; }
        public string UserType { get; internal set; } = "Normal";
        public string MSISDN { get; internal set; }
        public bool IsActive { get; set; } = true;
    }
}