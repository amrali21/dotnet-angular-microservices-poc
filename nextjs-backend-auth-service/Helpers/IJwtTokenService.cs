using nextjs_backend_auth_service.Models;

namespace nextjs_backend_auth_service.Helpers
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user);
    }
}
