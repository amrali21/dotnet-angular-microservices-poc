namespace ledgerly_backend.Models.FrontEnd
{
    public class InvoiceDetail
    {
        public string id { get; set; } = null!;
        public string? name { get; set; }
        public string customer_id { get; set; } = null!;
        public int amount { get; set; }
        public string status { get; set; } = null!;
    }
}
