using System.IO;
using System.Text;
using System.Threading.Tasks;
using Devjourney.Middlewares;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DevJourney.Tests.Middlewares
{
    public class FastETagMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_SetsETagHeader_ForSuccessfulGetRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/competitions";

            var middleware = new FastETagMiddleware(async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsync("test payload", Encoding.UTF8);
            });

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Response.Headers.ContainsKey("ETag"));
            var etag = context.Response.Headers.ETag.ToString();
            Assert.StartsWith("\"", etag);
            Assert.EndsWith("\"", etag);
        }

        [Fact]
        public async Task InvokeAsync_Returns304NotModified_WhenIfNoneMatchMatches()
        {
            // Arrange
            var payload = "cached data content";
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/scoreboard";

            // First run to get expected ETag
            string calculatedETag = string.Empty;
            var discoveryMiddleware = new FastETagMiddleware(async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsync(payload, Encoding.UTF8);
            });
            await discoveryMiddleware.InvokeAsync(context);
            calculatedETag = context.Response.Headers.ETag.ToString();

            // Second run with matching If-None-Match header
            var conditionalContext = new DefaultHttpContext();
            conditionalContext.Request.Method = "GET";
            conditionalContext.Request.Path = "/api/scoreboard";
            conditionalContext.Request.Headers["If-None-Match"] = calculatedETag;

            var conditionalMiddleware = new FastETagMiddleware(async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsync(payload, Encoding.UTF8);
            });

            // Act
            await conditionalMiddleware.InvokeAsync(conditionalContext);

            // Assert
            Assert.Equal(StatusCodes.Status304NotModified, conditionalContext.Response.StatusCode);
            Assert.Equal(0, conditionalContext.Response.ContentLength);
        }

        [Fact]
        public async Task InvokeAsync_DoesNotBufferOrSetETag_ForHubEndpoints()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/hubs/scoreboard";

            var middleware = new FastETagMiddleware(async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                await ctx.Response.WriteAsync("websocket handshake", Encoding.UTF8);
            });

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.False(context.Response.Headers.ContainsKey("ETag"));
        }
    }
}
