using Core.Entities;

namespace IntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task DeviceService_GetAllAsync_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = data.SampleOwner;
            var device = Device.Create(owner, deviceName);
            unitOfWork.DeviceCommandRepository.Add(device);
            await unitOfWork.SaveChangesAsync();
            
            // Act
            var fetchedDevices = await deviceService.GetAllAsync(new CancellationToken());
            
            // Assert
            Assert.Single(fetchedDevices);

            var fetchedDevice = fetchedDevices.First();
            
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
        
        [Fact]
        public async Task DeviceService_GetAllAsync_SinsssgleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = data.SampleOwner;
            var device = Device.Create(owner, deviceName);
            unitOfWork.DeviceCommandRepository.Add(device);
            await unitOfWork.SaveChangesAsync();
            
            // Act
            var fetchedDevices = await deviceService.GetAllAsync(new CancellationToken());
            
            // Assert
            Assert.Single(fetchedDevices);

            var fetchedDevice = fetchedDevices.First();
            
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
    }
}
