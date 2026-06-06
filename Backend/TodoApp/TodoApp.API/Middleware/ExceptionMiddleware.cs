using System.Net;
using System.Text.Json;

namespace TodoApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex.Message switch
            {
                var m when m.Contains("not found") => (int)HttpStatusCode.NotFound,
                var m when m.Contains("already exists") => (int)HttpStatusCode.BadRequest,
                var m when m.Contains("unauthorized") => (int)HttpStatusCode.Unauthorized,
                var m when m.Contains("forbidden") => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
