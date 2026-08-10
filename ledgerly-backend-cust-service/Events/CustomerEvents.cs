namespace ledgerly_backend_cust_service.Events
{
    public class CustomerCreatedEvent
    {
        public string CustomerId { get; set; } = null!;
    }

    public class CustomerDeletedEvent
    {
        public string CustomerId { get; set; } = null!;
    }
}
