using System.Net;
using System.Text.Json;
using DevFlow.Application.Exceptions;
namespace DevFlow.Api.Middleware
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
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unhandled exception occured");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            context.Response.ContentType = "application/json";
            if(ex is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }else if (ex is UnauthorizedException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
            }
            else if (ex is ConflictException)
            {
                statusCode = StatusCodes.Status409Conflict;
            }else if(ex is ValidationException validationException){
                statusCode = StatusCodes.Status400BadRequest;
                var response_ = new
                {
                    message = validationException.Message,
                    errors = validationException.Errors
                };
                context.Response.StatusCode = statusCode;
                return context.Response.WriteAsync(JsonSerializer.Serialize(response_));

            } else if(ex is ForbiddenException)
            {
                statusCode = StatusCodes.Status403Forbidden;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
            }
            context.Response.StatusCode = statusCode;

            var response = new
            {
                message = ex.Message,
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));

        }
    }
}
