using UserManagementAPI.Services;
using UserManagementAPI.Middleware;

namespace UserManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container
            builder.Services.AddControllers();

            // Add API documentation
            builder.Services.AddSwaggerGen();

            // Register User Service
            builder.Services.AddScoped<IUserService, UserService>();

            // Register JWT Token Service
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add health checks
            builder.Services.AddHealthChecks();

            // Build the application
            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
                    options.RoutePrefix = string.Empty;
                });
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Use HTTPS redirection
            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            // Add custom middleware in order
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<JwtAuthenticationMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // Authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map endpoints
            app.MapControllers();
            app.MapHealthChecks("/health");

            // Run the application
            app.Run();
        }
    }
}