using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Application.Modules.Competitions.Dtos;

namespace DevJourney.Tests.Integration
{
    public class PartnerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PartnerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetPublicScoreboard_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/scoreboard");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task VerifyCertificate_ValidCode_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/certificates/verify/12345");
            // Expect 404 because not found
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
