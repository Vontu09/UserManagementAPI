using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for validating JWT tokens and enforcing token-based authentication
    /// </summary>
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthenticationMiddleware> _logger;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        // Endpoints that bypass authentication
        private static readonly string[] AllowedPaths = new[] { "/health", "/api/auth/login", "/swagger", "/swagger-ui.html" };

        public JwtAuthenticationMiddleware(RequestDelegate next, ILogger<JwtAuthenticationMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            _jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            _jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for allowed paths
            if (IsAllowedPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var token = ExtractTokenFromHeader(context.Request.Headers);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No JWT token found in request to {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Missing or invalid authorization token" });
                return;
            }

            try
            {
                var principal = ValidateToken(token);
                context.User = principal;
                _logger.LogInformation("JWT token validated successfully for user: {UserId}", principal.FindFirst("sub")?.Value);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid JWT token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid or expired token" });
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Token validation failed" });
                return;
            }

            await _next(context);
        }

        private string? ExtractTokenFromHeader(IHeaderDictionary headers)
        {
            const string authorizationHeaderName = "Authorization";
            const string bearerScheme = "Bearer ";

            if (!headers.TryGetValue(authorizationHeaderName, out var authHeader))
            {
                return null;
            }

            var headerValue = authHeader.ToString();
            if (!headerValue.StartsWith(bearerScheme, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return headerValue[bearerScheme.Length..];
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }

        private bool IsAllowedPath(PathString path)
        {
            return AllowedPaths.Any(allowedPath =>
                path.StartsWithSegments(allowedPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}