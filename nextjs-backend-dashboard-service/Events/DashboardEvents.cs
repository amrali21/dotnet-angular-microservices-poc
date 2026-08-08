namespace nextjs_backend_dashboard_service.Events
{
    public class InvoiceCreatedEvent
    {
        public string InvoiceId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
    }

    public class InvoiceUpdatedEvent
    {
        public string InvoiceId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        // Amount is immutable after creation — only Status can change.
        public int Amount { get; set; }
        public string OldStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public DateTime Date { get; set; }
    }

    public class InvoiceDeletedEvent
    {
        public string InvoiceId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
    }

    public class CustomerCreatedEvent
    {
        public string CustomerId { get; set; } = null!;
    }

    public class CustomerDeletedEvent
    {
        public string CustomerId { get; set; } = null!;
    }
}
