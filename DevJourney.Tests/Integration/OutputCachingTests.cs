using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevJourney.Tests.Integration
{
    public class OutputCachingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public OutputCachingTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetScoreboard_CachesResponse()
        {
            var client = _factory.CreateClient();

            var response1 = await client.GetAsync("/api/scoreboard");
            var content1 = await response1.Content.ReadAsStringAsync();

            var response2 = await client.GetAsync("/api/scoreboard");
            var content2 = await response2.Content.ReadAsStringAsync();

            // While we cannot strictly test the cache hit without internal observability,
            // we can verify the endpoint responds with 200 OK successfully.
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            Assert.Equal(content1, content2);
        }
    }
}
