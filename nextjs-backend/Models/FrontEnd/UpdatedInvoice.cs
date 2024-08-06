namespace nextjs_backend.Models.FrontEnd
{
    public class UpdatedInvoice
    {
        public string id { get; set; }
        public string customerId { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
    }
}
