using AccessControl.Web;
using AccessControl.Web.StartupMethods;
using Application.Services.Devices;
using Application.UnitOfWorkStuff_to_sort;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ApiIntegrationTests.DevicesTests
{
    public static class Helper
    {
        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            services.RegisterApplicationServices();

            services.AddDbContext<AccessControlDbContext>(o => o.UseInMemoryDatabase($"local_test_db_{Guid.NewGuid().ToString()}"));

            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }
    }
    public class DevicesTests : IClassFixture<WebApplicationFactory<Program>>
    {

        public DevicesTests(WebApplicationFactory<Program> factory)
        {
            SetupApp();
        }

        private IUnitOfWork context;
        private HttpClient _httpClient;
        private void SetupApp()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task DeviceService()
        {
            var deviceService = Helper.GetRequiredService<IDeviceService>();
            var devices = await deviceService.GetAllAsync(new CancellationToken());

            Assert.Empty(devices);
        }
    }
}


/*

            var response = await _httpClient.GetAsync("/api/v1/devices/getAll");
            var streamec = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<IEnumerable<Device>>(streamec);

 */