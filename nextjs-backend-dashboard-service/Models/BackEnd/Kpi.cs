using System;
using System.Collections.Generic;

namespace nextjs_backend_dashboard_service.Models
{
    public partial class Kpi
    {
        public int ID { get; set; }
        public string KpiName { get; set; } = null!;
        public string? KpiDesc { get; set; }
        public int KpiValue { get; set; }
    }
}
