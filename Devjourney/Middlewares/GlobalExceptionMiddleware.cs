using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Devjourney.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var result = JsonSerializer.Serialize(new { message = ex.Message });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
