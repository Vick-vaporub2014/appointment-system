using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class ErrorHandlingMiddlewareService
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddlewareService> _logger;

        public ErrorHandlingMiddlewareService(RequestDelegate next, ILogger<ErrorHandlingMiddlewareService> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Detail = ex.Message
                };

                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));

            }
        }
    }
}
