namespace ledgerly_backend_cust_service.Models.FrontEnd
{
    public class PagedResult<T>
    {
        public List<T> data { get; set; } = new();
        public int count { get; set; }
    }
}
