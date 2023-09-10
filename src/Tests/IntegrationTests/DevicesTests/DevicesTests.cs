using Core.Entities.Devices;
using Framework.Extensions;

namespace IntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        // REGISTER NEW DEVICE METHOD WAS CHANGED TO ADD TO IOT HUB ETC. SO THIS TEST IS INVALID FOR NOW
        /*
        [Fact]
        public async Task DeviceService_RegisterNewDevice_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = data.SampleOwner;
            var device = Device.Create(owner, deviceName);
            await deviceService.RegisterNewDevice(device, new CancellationToken());

            // Act
            var fetchedDevice = await deviceService.GetByIdAsync(device.Id, new CancellationToken());
            
            // Assert
            Assert.NotNull(fetchedDevice);
            
            
            Assert.Equal(deviceName, fetchedDevice.Name);
            Assert.Equal(owner.Id, fetchedDevice.OwnerId);
            Assert.Equal(device.Type, fetchedDevice.Type);
        }
        */
        
        /*
            var iotHubServiceMock = new Mock<IIoTHubDeviceService>();
            iotHubServiceMock.Setup(s => s
                .RegisterDeviceToIotHub(It.IsAny<Guid>())
            ).Returns(Task.CompletedTask);

            var unitofWorkMock = new Mock<IUnitOfWork>();
            unitofWorkMock.Setup(e => e.DeviceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(device));

            var deviceService = new DeviceService(unitOfWork: unitofWorkMock.Object,ioTHubDeviceService: iotHubServiceMock.Object);
         */

        [Fact]
        public async Task DeviceService_GetByOwnerId_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var owner = await GetOwner();
            var deviceName = "mujdevice";
            var device = Device.Create(owner, deviceName, DeviceType.Lock);
            
            await deviceService.RegisterNewDevice(device, new CancellationToken());

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
            var owner = await GetOwner();
            
            var deviceName1 = "mujdevice1";
            var device1 = Device.Create(owner, deviceName1, DeviceType.Lock);
            
            var deviceName2 = "mujdevice2";
            var device2 = Device.Create(owner, deviceName2, DeviceType.Lock);

            var deviceName3 = "mujdevice3";
            var device3 = Device.Create(owner, deviceName3, DeviceType.Lock);

            await deviceService.RegisterNewDevice(device1, new CancellationToken());
            await deviceService.RegisterNewDevice(device2, new CancellationToken());
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
            
            Assert.Equal(deviceName3, fetchedDevicesList[2].Name);
            Assert.Equal(owner.Id, fetchedDevicesList[2].OwnerId);
            Assert.Equal(device3.Type, fetchedDevicesList[2].Type);
        }
        
        
        [Fact]
        public async Task DeviceService_GetAllAsync_SingleDevice_Query_Succeeds()
        {
            // Arrange
            var deviceName = "mujdevice";
            var owner = await GetOwner();
            var device = Device.Create(owner, deviceName, DeviceType.Lock);
            await deviceService.RegisterNewDevice(device, new CancellationToken());
            
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
