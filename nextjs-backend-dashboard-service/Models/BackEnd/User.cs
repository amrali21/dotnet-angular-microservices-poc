using System;
using System.Collections.Generic;

namespace nextjs_backend_dashboard_service.Models
{
    public partial class User
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
