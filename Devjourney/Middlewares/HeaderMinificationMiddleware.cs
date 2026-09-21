using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Devjourney.Middlewares
{
    public class HeaderMinificationMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMinificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers.Remove("Server");
                headers.Remove("X-Powered-By");
                headers.Remove("X-AspNet-Version");
                headers.Remove("X-AspNetMvc-Version");
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
