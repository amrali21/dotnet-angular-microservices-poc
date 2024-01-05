using System;
using System.Collections.Generic;

namespace nextjs_backend.Models
{
    public partial class Invoice
    {
        public string Id { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
