using Core.Entities;
using IntegrationTests;
using Newtonsoft.Json;

namespace ApiIntegrationTests
{
    public class DevicesApiTests : AccessControlIntegrationTests
    {
        // testy pro api jsou furt work in progress
        [Test]
        public async Task Devices_Get_All_Returns_2xx()
        {
            var response = await _httpClient.GetAsync("/api/v1/devices/getAll");
            var streamec = await response.Content.ReadAsStreamAsync();
            // var model = JsonSerializer.Deserialize<IEnumerable<Device>>(streamec);

            response.EnsureSuccessStatusCode();
        }
    }
}
