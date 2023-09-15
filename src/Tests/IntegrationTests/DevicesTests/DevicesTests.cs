using Core.Entities.Devices;
using Framework.Extensions;

namespace IntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task DeviceService_GetByOwnerId_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var owner = await GetOwnerAsync();
            var deviceName = "mujdevice";
            var device = Device.Create(owner, deviceName, DeviceType.Lock);
            
            await deviceService.RegisterNewDeviceAsync(device, new CancellationToken());
            await deviceService.SaveChangesAsync(new CancellationToken());

            // Act
            var fetchedDevices = await deviceService.GetByOwnerIdAsync(owner.Id, new CancellationToken());
            var fetchedDevice = fetchedDevices.First();

            // Assert
            Assert.NotNull(fetchedDevice);

            Assert.Single(fetchedDevices);
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
        
        [Fact]
        public async Task DeviceService_GetByOwnerId_MultipleDevices_SingleOwner_Query_Succeeds()
        {
            // Arrange
            var owner = await GetOwnerAsync();
            
            var deviceName1 = "mujdevice1";
            var device1 = Device.Create(owner, deviceName1, DeviceType.Lock);
            
            var deviceName2 = "mujdevice2";
            var device2 = Device.Create(owner, deviceName2, DeviceType.Lock);

            var deviceName3 = "mujdevice3";
            var device3 = Device.Create(owner, deviceName3, DeviceType.Lock);

            await deviceService.RegisterNewDeviceAsync(device1, new CancellationToken());
            await deviceService.RegisterNewDeviceAsync(device2, new CancellationToken());
            await deviceService.RegisterNewDeviceAsync(device3, new CancellationToken());
            await deviceService.SaveChangesAsync(new CancellationToken());

            // Act
            var fetchedDevices = await deviceService.GetByOwnerIdAsync(owner.Id, new CancellationToken());
            var fetchedDevicesList = fetchedDevices.ToList();
            
            // Assert
            Assert.Equal(3, fetchedDevicesList.Count);

            foreach (var device in fetchedDevicesList)
            {
                Assert.True(deviceName1 == device.Name || deviceName2 == device.Name || deviceName3 == device.Name);
                Assert.True(device.Type == DeviceType.Lock);
                Assert.True(owner.Id == device.OwnerId);
            }
        }
        
        
        [Fact]
        public async Task DeviceService_GetAllAsync_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = await GetOwnerAsync();
            var device = Device.Create(owner, deviceName, DeviceType.Lock);
            await deviceService.RegisterNewDeviceAsync(device, new CancellationToken());
            await deviceService.SaveChangesAsync(new CancellationToken());
            
            // Act
            var fetchedDevices = (await deviceService.GetByIdAsync(device.Id, new CancellationToken())).ToEnumerable();
            
            // Assert
            Assert.Single(fetchedDevices);

            var fetchedDevice = fetchedDevices.First();
            
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
    }
}
