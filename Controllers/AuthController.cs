using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtTokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Generate JWT token for API authentication
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Username and password are required" });
                }

                // For demo purposes, accept any non-empty username/password
                // In production, validate against a database
                if (request.Username.Length < 3 || request.Password.Length < 6)
                {
                    return BadRequest(new { message = "Username must be at least 3 characters and password at least 6 characters" });
                }

                var token = _tokenService.GenerateToken(request.Username, $"{request.Username}@example.com");
                var expirationMinutes = _tokenService.GetTokenExpirationMinutes();

                _logger.LogInformation("Login successful for user: {Username}", request.Username);

                return Ok(new
                {
                    accessToken = token,
                    tokenType = "Bearer",
                    expiresIn = expirationMinutes * 60 // in seconds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Login failed" });
            }
        }
    }

    /// <summary>
    /// Login request model
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}