using Microsoft.AspNetCore.Identity;

namespace ledgerly_backend_auth_service.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
