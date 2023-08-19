using System.Text.Json;
using AccessControl.Web;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiIntegrationTests.DevicesTests
{
    public class DevicesTests : IClassFixture<WebApplicationFactory<Program>>
    {

        public DevicesTests(WebApplicationFactory<Program> factory)
        {
            SetupApp();
        }

        private HttpClient _httpClient;
        private void SetupApp()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            Environment.SetEnvironmentVariable("ConnectToTestDatabase", "true");
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task Devices_GetAll_Full()
        {
            var response = await _httpClient.GetAsync("/api/v1/devices/getAll");
            var streamec = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<IEnumerable<Device>>(streamec);
            
            response.EnsureSuccessStatusCode();
        }
    }
}
