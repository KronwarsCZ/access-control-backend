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
        // User shouldn't see whats wrong (that's only for our logging purposes) -> user gets a 400 BadRequest error
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        
        // TODO: Why does it return error?
        //Assert.True(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
        Assert.False(secondEndpointResponse.HttpResponse.IsSuccessStatusCode);
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
            OwnerId = Guid.NewGuid(), // this should be unique - not existing in db 
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
            DeviceId = Guid.NewGuid(),
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(_attachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyRole_Fail()
    {
        // Arrange
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
        Assert.False(endpointResponse.HttpResponse.IsSuccessStatusCode);
    }
}
