using AccessControl.Web;
using AccessControl.Web.StartupMethods;
using Application.Services.Devices;
using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Identification;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ApiIntegrationTests.DevicesTests
{
    public class DevicesTests : AccessControlIntegrationTests
    {
        [Fact]
        public async Task DeviceAddAndRetrieve()
        {
            // Arrange
            var deviceName = "mujdevice";
            var device = deviceService.Create(data.SampleOwner, deviceName);
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