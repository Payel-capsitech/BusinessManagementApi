using BusinessManagementApi.DTOs;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagementApi.Controllers
{
    /// <summary>
    /// Handles user registration and login APIs.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Injects the AuthService.
        /// </summary>
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with the provided data.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if credentials are valid.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { token });
        }

        [HttpGet("userdetails")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token or missing user ID." });

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                role = user.Role
            });
        }


    }
}