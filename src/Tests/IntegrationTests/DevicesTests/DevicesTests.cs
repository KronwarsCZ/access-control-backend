using Core.Entities;

namespace IntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task DeviceService_RegisterNewDevice_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = data.SampleOwner;
            var device = Device.Create(owner, deviceName);
            deviceService.RegisterNewDevice(device, new CancellationToken());

            // Act
            var fetchedDevice = await deviceService.GetByIdAsync(device.Id, new CancellationToken());
            
            // Assert
            Assert.NotNull(fetchedDevice);
            
            
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
        
        [Fact]
        public async Task DeviceService_GetByOwnerId_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = data.SampleOwner;
            var device = Device.Create(owner, deviceName);
            deviceService.RegisterNewDevice(device, new CancellationToken());

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
        
        // PROBLEM WITH ONE OWNER FOR MANY DEVICES
        /*
        [Fact]
        public async Task DeviceService_GetByOwnerId_MultipleDevices_SingleOwner_Query_Succeeds()
        {
            // Arrange
            var owner = data.SampleOwner;
            
            var deviceName1 = "mujdevice1";
            var device1 = Device.Create(owner, deviceName1);
            await deviceService.RegisterNewDevice(device1, new CancellationToken());
            
            var deviceName2 = "mujdevice2";
            var device2 = Device.Create(owner, deviceName2);
            await deviceService.RegisterNewDevice(device2, new CancellationToken());
            
            var deviceName3 = "mujdevice3";
            var device3 = Device.Create(owner, deviceName3);
            await deviceService.RegisterNewDevice(device3, new CancellationToken());

            // Act
            var fetchedDevices = await deviceService.GetByOwnerIdAsync(owner.Id, new CancellationToken());
            var fetchedDevicesList = fetchedDevices.ToList();
            
            // Assert
            Assert.Equal(3, fetchedDevicesList.Count());
            
            Assert.Equal(deviceName1, fetchedDevicesList[0].Name);
            Assert.Equal(owner.Id, fetchedDevicesList[0].OwnerId);
            Assert.Equal(device1.Type, fetchedDevicesList[0].Type);
            
            Assert.Equal(deviceName2, fetchedDevicesList[1].Name);
            Assert.Equal(owner.Id, fetchedDevicesList[1].OwnerId);
            Assert.Equal(device2.Type, fetchedDevicesList[1].Type);
            
            Assert.Equal(deviceName2, fetchedDevicesList[2].Name);
            Assert.Equal(owner.Id, fetchedDevicesList[2].OwnerId);
            Assert.Equal(device2.Type, fetchedDevicesList[2].Type);
        }
        */
        
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
    }
}
