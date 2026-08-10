using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ledgerly_backend_auth_service.Helpers;
using ledgerly_backend_auth_service.Models;
using ledgerly_backend_auth_service.Models.Dtos;

namespace ledgerly_backend_auth_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    DisplayName = request.Name
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(e => e.Description));
                }

                return Ok(BuildAuthResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register user {Email}", request.Email);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Unauthorized();
                }

                return Ok(BuildAuthResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed login attempt for {Email}", request.Email);
                return StatusCode(500);
            }
        }

        private AuthResponse BuildAuthResponse(ApplicationUser user)
        {
            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                User = new AuthUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    DisplayName = user.DisplayName
                }
            };
        }
    }
}
