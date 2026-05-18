namespace nextjs_backend.Models.FrontEnd
{
    public class InsertedInvoice
    {
        public string customerId { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
    }
}
