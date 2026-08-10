namespace ledgerly_backend.Models.FrontEnd
{
    public class InvoiceListItem
    {
        public string id { get; set; } = null!;
        public int amount { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; } = null!;
        public string name { get; set; } = null!;
        public string email { get; set; } = null!;
        public string image_url { get; set; } = null!;
    }
}
