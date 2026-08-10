using ledgerly_backend_auth_service.Models;

namespace ledgerly_backend_auth_service.Helpers
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user);
    }
}
