using System.Text.Json;
using Application.IRepositories;
using Core.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiIntegrationTests
{
    public class DevicesTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private WebApplicationFactory<Program> _factory;
        private SampleDataService data;
        private HttpClient _httpClient;

        public DevicesTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateDefaultClient();
        }


        private void SetupApp()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            Environment.SetEnvironmentVariable("ConnectToTestDatabase", "true");
            _httpClient = webApplicationFactory.CreateDefaultClient();
            data = webApplicationFactory.Services.GetRequiredService<SampleDataService>();
        }

        [Fact]
        public async Task Devices_GetAll_Full()
        {
            var response = await _httpClient.GetAsync("/api/v1/devices/getAll");

            
            response.EnsureSuccessStatusCode();
        }
    }
}
