using System.Text.Json;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for enforcing standardized error handling across all endpoints
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ArgumentNullException argNullEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = 400;
                    response.Message = "Bad Request";
                    response.Details = argNullEx.Message;
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.StatusCode = 400;
                    response.Message = "Bad Request";
                    response.Details = argEx.Message;
                    break;

                case InvalidOperationException invOpEx:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.StatusCode = 409;
                    response.Message = "Conflict";
                    response.Details = invOpEx.Message;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.StatusCode = 401;
                    response.Message = "Unauthorized";
                    response.Details = "You are not authorized to access this resource";
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.StatusCode = 500;
                    response.Message = "Internal Server Error";
                    response.Details = "An unexpected error occurred. Please try again later";
                    break;
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsJsonAsync(response, jsonOptions);
        }
    }

    /// <summary>
    /// Standardized error response model
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}