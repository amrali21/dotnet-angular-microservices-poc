using System;
using System.Collections.Generic;

namespace nextjs_backend_dashboard_service.Models
{
    public partial class Kpi
    {
        public int TotalBilled { get; set; }
        public int Collected { get; set; }
        public int Outstanding { get; set; }
        public int Customers { get; set; }
    }
}
