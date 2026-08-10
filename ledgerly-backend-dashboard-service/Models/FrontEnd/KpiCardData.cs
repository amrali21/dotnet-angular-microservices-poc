namespace ledgerly_backend_dashboard_service.Models.FrontEnd
{
    public class KpiCardData
    {
        public int totalBilled { get; set; }
        public int collected { get; set; }
        public int outstanding { get; set; }
        public int customers { get; set; }
    }
}
