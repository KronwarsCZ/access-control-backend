using Core.Entities;

namespace ApiIntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task DeviceAddAndRetrieve()
        {
            // Arrange
            var deviceName = "mujdevice";
            var device = Device.Create(data.SampleOwner, deviceName);
            unitOfWork.DeviceCommandRepository.Add(device);
            await unitOfWork.SaveChangesAsync();
            
            // Act
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