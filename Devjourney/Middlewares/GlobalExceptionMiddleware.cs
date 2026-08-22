using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Devjourney.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Logging.ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, Microsoft.Extensions.Logging.ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                try
                {
                    await _next(context);
                }
                catch (NotFoundException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    var result = JsonSerializer.Serialize(new { message = ex.Message });
                    await context.Response.WriteAsync(result);
                }
                catch (FluentValidation.ValidationException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    var result = JsonSerializer.Serialize(new { message = "Validation failed", errors });
                    await context.Response.WriteAsync(result);
                }
                catch (BadRequestException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var result = JsonSerializer.Serialize(new { message = ex.Message, errors = ex.Errors });
                    await context.Response.WriteAsync(result);
                }
                catch (UnauthorizedException ex)
                {
                    _logger.LogWarning(ex, "Unauthorized access attempt.");
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    var result = JsonSerializer.Serialize(new { message = ex.Message });
                    await context.Response.WriteAsync(result);
                }
                catch (ForbiddenAccessException ex)
                {
                    _logger.LogWarning(ex, "Forbidden access attempt.");
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    var result = JsonSerializer.Serialize(new { message = ex.Message });
                    await context.Response.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unhandled exception has occurred.");
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    
                    var responseBody = new 
                    { 
                        message = "An internal server error occurred.", 
                        correlationId = correlationId 
                    };
                    
                    var result = JsonSerializer.Serialize(responseBody);
                    await context.Response.WriteAsync(result);
                }
            } // Close using scope

            if (!context.Response.HasStarted && context.Response.StatusCode >= 400 && (string.IsNullOrEmpty(context.Response.ContentType) || context.Response.ContentLength == null || context.Response.ContentLength == 0))
            {
                context.Response.ContentType = "application/json";
                var status = context.Response.StatusCode;
                var title = status switch
                {
                    400 => "Bad Request",
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "Not Found",
                    415 => "Unsupported Media Type",
                    _ => "An error occurred"
                };
                var payload = JsonSerializer.Serialize(new { success = false, error = new { code = title.ToUpper().Replace(" ", "_"), message = title } });
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
