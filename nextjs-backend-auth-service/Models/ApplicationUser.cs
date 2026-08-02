using Microsoft.AspNetCore.Identity;

namespace nextjs_backend_auth_service.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
