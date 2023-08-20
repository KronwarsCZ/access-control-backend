using AccessControl.Web;
using AccessControl.Web.StartupMethods;
using Application.Services.Devices;
using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Identification;
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

            services.AddDbContext<AccessControlDbContext>(o =>
                o.UseInMemoryDatabase($"local_test_db_{Guid.NewGuid().ToString()}"),
                ServiceLifetime.Transient
            );

            
            /*
            var connectionString = @"Server=.\\SQLEXPRESS;Database=access-control-test;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true";
            
            services.AddDbContext<AccessControlDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    optionsAction => optionsAction.EnableRetryOnFailure()
                ).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );
            */
            return services.BuildServiceProvider();
        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();
            return provider.GetRequiredService<T>();
        }
    }

    public class DataGenerator
    {
        public DataGenerator()
        {
            SampleOwner = new Owner
            {
                Id = SequentialGuid.Create(),
                Name = "Sample Test Owner",
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                IsDeleted = false,
            };
        }

        public Owner SampleOwner;
    }
    public class DevicesTests : IClassFixture<WebApplicationFactory<Program>>
    {

        public DevicesTests(WebApplicationFactory<Program> factory)
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
            context = Helper.GetRequiredService<IUnitOfWork>();
        }

        private IUnitOfWork context;
        private HttpClient _httpClient;

        [Fact]
        public async Task DeviceAddAndRetrieve()
        {
            // Arrange
            var dataGenerator = new DataGenerator();
            var deviceService = Helper.GetRequiredService<IDeviceService>();

            var deviceName = "mujdevice";
            var device = deviceService.Create(dataGenerator.SampleOwner, deviceName);
            context.DeviceCommandRepository.Add(device);
            await context.SaveChangesAsync();

            // Act
            var devices = await context.DeviceQueryRepository.GetAllAsync(new CancellationToken());

            // Assert
            Assert.Single(devices);
        }
        
        [Fact]
        public async Task DeviceService_GetAll()
        {
            // Arrange
            var dataGenerator = new DataGenerator();

            var deviceName = "mujdevice";
            var device =  new Device
            {
                Id = SequentialGuid.Create(),
                Name = "muj super device",
                Type = DeviceType.Lock,
                Owner = dataGenerator.SampleOwner,
                OwnerId = dataGenerator.SampleOwner.Id,
                IsDeleted = false,
                CreatedUtc = context.TimeService.GetUtcTime(),
                UpdatedUtc = context.TimeService.GetUtcTime()
            };
            context.DeviceCommandRepository.Add(device);
            await context.SaveChangesAsync();

            // Act
            var deviceService = Helper.GetRequiredService<IDeviceService>();
            var devices = await deviceService.GetAllAsync(new CancellationToken());

            // Assert
            Assert.Single(devices);
        }
    }
}


/*
    var response = await _httpClient.GetAsync("/api/v1/devices/getAll");
    var streamec = await response.Content.ReadAsStringAsync();
    var model = JsonSerializer.Deserialize<IEnumerable<Device>>(streamec);
 */