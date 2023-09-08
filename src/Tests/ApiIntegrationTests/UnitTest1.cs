using IntegrationTests;
using Microsoft.Extensions.DependencyInjection;

namespace ApiIntegrationTests
{
    public class DevicesApiTests : AccessControlIntegrationTests
    {
        // testy pro api jsou furt work in progress
        [Fact]
        public async Task Devices_Get_All_Returns_2xx()
        {
            var response = await _httpClient.GetAsync("/api/v1/devices/getAll");
            var streamec = await response.Content.ReadAsStreamAsync();
            // var model = JsonSerializer.Deserialize<IEnumerable<Device>>(streamec);

            response.EnsureSuccessStatusCode();
        }
    }
}
