using System;
using System.Collections.Generic;

namespace ledgerly_backend_dashboard_service.Models
{
    public partial class Revenue
    {
        public string Month { get; set; } = null!;
        public int Year { get; set; }
        public int Revenue1 { get; set; }
    }
}
