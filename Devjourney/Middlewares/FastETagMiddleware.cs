using System;
using System.IO;
using System.IO.Hashing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Devjourney.Middlewares
{
    public class FastETagMiddleware
    {
        private readonly RequestDelegate _next;

        public FastETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only process GET and HEAD requests
            var method = context.Request.Method;
            if (!HttpMethods.IsGet(method) && !HttpMethods.IsHead(method))
            {
                await _next(context);
                return;
            }

            // Skip WebSockets and SignalR hub endpoints
            var path = context.Request.Path;
            if (context.WebSockets.IsWebSocketRequest || path.StartsWithSegments("/hubs"))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            await using var bufferStream = new MemoryStream();
            context.Response.Body = bufferStream;

            try
            {
                await _next(context);

                // Only evaluate ETags for successful 200 OK responses with body content
                if (context.Response.StatusCode == StatusCodes.Status200OK && bufferStream.Length > 0)
                {
                    // Check if response is streaming or event-stream, in which case ETag should not buffer
                    var contentType = context.Response.ContentType ?? string.Empty;
                    if (contentType.Contains("text/event-stream", StringComparison.OrdinalIgnoreCase) ||
                        contentType.Contains("application/x-ndjson", StringComparison.OrdinalIgnoreCase))
                    {
                        bufferStream.Position = 0;
                        await bufferStream.CopyToAsync(originalBodyStream);
                        return;
                    }

                    // Compute SIMD-accelerated XxHash64 of response bytes
                    var buffer = bufferStream.ToArray();
                    var hashBytes = XxHash64.Hash(buffer);
                    var etag = $"\"{Convert.ToHexString(hashBytes)}\"";

                    context.Response.Headers.ETag = etag;

                    // Evaluate If-None-Match conditional request
                    if (context.Request.Headers.TryGetValue("If-None-Match", out var clientETag) &&
                        string.Equals(clientETag.ToString().Trim(), etag, StringComparison.Ordinal))
                    {
                        context.Response.StatusCode = StatusCodes.Status304NotModified;
                        context.Response.ContentLength = 0;
                        return; // Send 0 bytes payload
                    }

                    bufferStream.Position = 0;
                    await bufferStream.CopyToAsync(originalBodyStream);
                }
                else
                {
                    if (bufferStream.Length > 0)
                    {
                        bufferStream.Position = 0;
                        await bufferStream.CopyToAsync(originalBodyStream);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
