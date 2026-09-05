using System.Text;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for logging all incoming requests and outgoing responses for auditing purposes
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Store original body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                // Log incoming request
                await LogRequestAsync(context);

                // Create a memory stream to capture response
                using (var memoryStream = new MemoryStream())
                {
                    context.Response.Body = memoryStream;

                    // Call the next middleware
                    await _next(context);

                    // Log outgoing response
                    await LogResponseAsync(context, memoryStream);

                    // Copy the memory stream to the original response body
                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the request/response logging middleware");
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequestAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var request = context.Request;
            var bodyContent = string.Empty;

            if (request.ContentLength > 0)
            {
                try
                {
                    using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                    {
                        bodyContent = await reader.ReadToEndAsync();
                        request.Body.Position = 0; // Reset stream position for subsequent reads
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read request body");
                }
            }

            _logger.LogInformation(
                "HTTP Request - Method: {HttpMethod}, Path: {Path}, Query: {Query}, " +
                "ContentType: {ContentType}, SourceIP: {SourceIP}, Body: {Body}",
                request.Method,
                request.Path,
                request.QueryString,
                request.ContentType,
                context.Connection.RemoteIpAddress,
                string.IsNullOrEmpty(bodyContent) ? "No body" : bodyContent);
        }

        private async Task LogResponseAsync(HttpContext context, MemoryStream memoryStream)
        {
            var response = context.Response;
            memoryStream.Position = 0;

            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Position = 0;

            _logger.LogInformation(
                "HTTP Response - StatusCode: {StatusCode}, Path: {Path}, " +
                "ContentType: {ContentType}, Body: {Body}",
                response.StatusCode,
                context.Request.Path,
                response.ContentType,
                string.IsNullOrEmpty(responseBody) ? "No body" : responseBody);
        }
    }
}