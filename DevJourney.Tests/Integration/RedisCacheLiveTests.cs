using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevJourney.Tests.Integration
{
    public class RedisCacheLiveTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public RedisCacheLiveTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task RedisCloud_DistributedCache_CanSetAndGetValues()
        {
            using var scope = _factory.Services.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

            // Ensure we are using RedisCache, not memory cache
            var cacheTypeName = cache.GetType().FullName;
            Assert.Contains("RedisCache", cacheTypeName);

            var testKey = $"test_redis_key_{Guid.NewGuid():N}";
            var testValue = $"Hello Redis Cloud at {DateTime.UtcNow:O}";
            var testBytes = Encoding.UTF8.GetBytes(testValue);

            // Set
            await cache.SetAsync(testKey, testBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            });

            // Get
            var retrievedBytes = await cache.GetAsync(testKey);
            Assert.NotNull(retrievedBytes);

            var retrievedValue = Encoding.UTF8.GetString(retrievedBytes);
            Assert.Equal(testValue, retrievedValue);

            // Clean up
            await cache.RemoveAsync(testKey);
            var afterRemove = await cache.GetAsync(testKey);
            Assert.Null(afterRemove);
        }
    }
}
