using Core.Entities;

namespace IntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Test]
        public async Task DeviceAddAndRetrieve()
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
            Assert.AreEqual(1, fetchedDevices.Count());

            var fetchedDevice = fetchedDevices.First();
            
            Assert.AreEqual(deviceName, fetchedDevice.Name);
            Assert.AreEqual(owner.Id, fetchedDevice.Owner.Id);
            Assert.AreEqual(device.Type, fetchedDevice.Type);
        }
    }
}
