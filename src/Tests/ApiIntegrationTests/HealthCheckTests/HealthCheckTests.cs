using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiIntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private HttpClient _httpClient;
        public HealthCheckTests()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }
        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {

            var response = await _httpClient.GetAsync("/healthcheck");

            response.EnsureSuccessStatusCode();
        }
    }
}
