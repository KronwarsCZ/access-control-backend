using System.Net;
using AccessControl.Web.Api.V1.Dto;
using AccessControl.Web.Api.V1.Dto.Devices;
using Core.Entities.Devices;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class DevicesApiTests : ApiTestsSetup
{
    private readonly Uri _registerNewDeviceUri = new("api/v1/devices/registerNewDevice", UriKind.Relative);
    
    [Fact]
    public async Task RegisterNewDevice_Success()
    {
        // Arrange
        var owner = await GetOwner();
        var name = "test device";
        var payload = new DeviceAddParameters()
        {
            OwnerId = owner.Id,
            Name = name
        };
        
        // Act
        var endpointResponse = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(_registerNewDeviceUri, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();

        var responseId = endpointResponse.DeserializedResponse.DeviceId;
        var deviceFromDb = await deviceService.GetByIdAsync(responseId, new CancellationToken());

        // Assert
        Assert.NotNull(deviceFromDb);
        Assert.Equal(owner.Id, deviceFromDb.OwnerId);
        Assert.Equal(name, deviceFromDb.Name);
        Assert.Equal(DeviceType.Lock, deviceFromDb.Type);
    }

    [Fact]
    public async Task RegisterNewDevice_WrongParameters_EmptyGuid()
    {
        // Arrange
        const string name = "test device";
        var payload = new DeviceAddParameters()
        {
            Name = name
        };
        
        // Act
        var endpointResponse = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(_registerNewDeviceUri, HttpMethod.Post, payload);

        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task RegisterNewDevice_WrongParameters_NonExistingGuid()
    {
        // Arrange
        var ownerId = new Guid(); // guid has all 0 - shouldn't exist in db
        const string name = "test device";
        var payload = new DeviceAddParameters()
        {
            OwnerId = ownerId,
            Name = name
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<DeviceAddParameters, DeviceAddResult>(_registerNewDeviceUri, HttpMethod.Post, payload);

        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task RegisterNewDevice_WrongParameters_InvalidName()
    {
        // Arrange
        var owner = await GetOwner();
        const string name = "        \n \t";
        var payload = new DeviceAddParameters()
        {
            OwnerId = owner.Id,
            Name = name
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<DeviceAddParameters, DeviceAddResult>(_registerNewDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Name", errorMessage);
    }

    private readonly Uri _attachRoleToDeviceUri = new("api/v1/devices/attachRoleToDevice", UriKind.Relative);

    [Fact]
    public async Task AttachRoleToDevice_Success()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.True(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }
    
    [Fact]
    public async Task AttachRoleToDevice_AlreadyAttached_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        var secondEndpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        
        Assert.True(secondEndpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await secondEndpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(device.Id.ToString(), errorMessage);
        Assert.Contains(role.Id.ToString(), errorMessage);
        Assert.True(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }

    [Fact]
    public async Task AttachRoleToDevice_WrongOwner_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = new Guid(), // this should be all zeroes 
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
        Assert.False(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyOwner_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
        Assert.False(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }

    [Fact]
    public async Task AttachRoleToDevice_WrongDevice_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = new Guid(),
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyDevice_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task AttachRoleToDevice_WrongRole_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);

        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = Guid.NewGuid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyRole_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);

        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
    }

    [Fact]
    public async Task AttachRoleToDevice_DeviceDoesntBelongToOwner_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var owner2 = await GetOwner();
        var device = await GetDevice(owner2);
        var role = await GetRole(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
        Assert.False(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }
    
    [Fact]
    public async Task AttachRoleToDevice_RoleDoesntBelongToOwner_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var owner2 = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner2);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
        Assert.False(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }

    private readonly Uri _getAllRolesForDeviceUri = new Uri("api/v1/devices/getAllRolesForDevice", UriKind.Relative);

    [Fact]
    public async Task GetAllRolesForDevice_DeviceHas1Role_Success()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role = await GetRole(owner);

        var attachPayload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Attach Role to Device
        var attachResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, attachPayload);
        attachResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };

        var expectedResult = new RoleResult()
        {
            Id = role.Id,
            Name = "test role"
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var roles = endpointResponse.DeserializedResponse.Roles.ToArray();
        Assert.NotNull(roles);
        Assert.True(roles.Length == 1);
        Assert.Equal(expectedResult.Id, roles[0].Id);
        Assert.Equal(expectedResult.Name, roles[0].Name);
    }

    [Fact]
    public async Task GetAllRolesForDevice_DeviceHasMultipleRoles_Success()
    {
        // Arrange
        var owner = await GetOwner();
        var device = await GetDevice(owner);
        var role1 = await GetRole(owner);
        
        
        var attachPayload1 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role1.Id
        };
        
        var attachResponse1 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, attachPayload1);
        attachResponse1.HttpResponse.EnsureSuccessStatusCode();
        
        
        var role2 = await GetRole(owner);
        
        var attachPayload2 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role2.Id
        };
        
        var attachResponse2 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, attachPayload2);
        attachResponse2.HttpResponse.EnsureSuccessStatusCode();


        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };

        var expectedResult = new GetAllRolesForDeviceResult()
        {
            Roles = new[] 
                { 
                    new RoleResult() {Id = role1.Id, Name = "test role"}, 
                    new RoleResult() {Id = role2.Id, Name = "test role"}
                }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var roles = endpointResponse.DeserializedResponse.Roles.ToArray();
        Assert.NotNull(roles);
        Assert.True(roles.Length == 2);
        
        // For each expected role loop through all result roles to check if there is one with the correct id
        // Assert.Contains doesnt work :/
        foreach (var expectedRole in expectedResult.Roles)
        {
            var found = false;
            foreach (var roleResult in roles)
            {
                if (expectedRole.Id == roleResult.Id && roleResult.Name == "test role") found = true;
            }
            Assert.True(found);
        }
    }

    [Fact]
    public async Task GetAllRolesForDevice_EmptyOwner_Fail()
    {
        // Arrange
        var payload = new GetAllRolesForDeviceParameters()
        {
            DeviceId = new Guid()
        };

        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);

    }

    [Fact]
    public async Task GetAllRolesForDevice_NonexistingOwner_Fail()
    {
        // Arrange
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = new Guid(),
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task GetAllRolesForDevice_EmptyDevice_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task GetAllRolesForDevice_NonexistingDevice_Fail()
    {
        // Arrange
        var owner = await GetOwner();
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);

    }

    [Fact]
    public async Task GetAllRolesForDevice_DeviceWrongOwner_Fail()
    {
        // Arrange
        var owner1 = await GetOwner();
        
        var owner2 = await GetOwner();
        var device = await GetDevice(owner2);
        
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner1.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(_getAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }


    private readonly Uri _getOpeningKeysUri = new Uri("api/v1/devices/getOpeningKeys", UriKind.Relative);
    
    [Fact]
    public async Task 1()
    {
        // Arrange

        // Act

        // Assert

    }
}
