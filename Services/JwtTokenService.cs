using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementAPI.Services
{
    /// <summary>
    /// Service for generating and managing JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, string email);
        int GetTokenExpirationMinutes();
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenService> _logger;

        public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string userId, string email)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
            var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

            var key = Encoding.ASCII.GetBytes(jwtSecret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("email", email),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("JWT token generated for user: {UserId}", userId);
            return tokenString;
        }

        public int GetTokenExpirationMinutes()
        {
            return int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        }
    }
}