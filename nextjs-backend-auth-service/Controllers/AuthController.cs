using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nextjs_backend_auth_service.Helpers;
using nextjs_backend_auth_service.Models;
using nextjs_backend_auth_service.Models.Dtos;

namespace nextjs_backend_auth_service.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
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

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized();
            }

            return Ok(BuildAuthResponse(user));
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
