using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Application.Modules.Competitions.Dtos;
using Microsoft.Extensions.DependencyInjection;

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
        public async Task PublishCompetitionJ_AndVerifyInAvailableCompetitions()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataAccessLayer.DataContexts.DataContext>();
                var targetComp = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                    db.Competitions, c => c.Title == "j");
                if (targetComp != null)
                {
                    targetComp.IsPublished = true;
                    await db.SaveChangesAsync();
                }
            }

            var response = await _client.GetAsync("/api/competitions");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("\"title\":\"j\"", json);
        }
    }
}
