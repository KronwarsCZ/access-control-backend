using System.Net;
using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using ApiShared.Dto.Roles;
using Core.Entities.Devices;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class DevicesApiTests : ApiTestsSetup
{
    [Fact]
    public async Task RegisterNewDevice_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var name = "test device";
        var payload = new DeviceAddParameters()
        {
            OwnerId = owner.Id,
            Name = name
        };
        
        // Act
        var endpointResponse = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);
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
    public async Task RegisterNewDevice_RegisterNewDeviceAgain_What()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var name = "test device";
        var payload = new DeviceAddParameters()
        {
            OwnerId = owner.Id,
            Name = name
        };
        
        // Act
        var endpointResponse = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();

        var endpointResponse2 = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);
        endpointResponse2.HttpResponse.EnsureSuccessStatusCode();
        
        var responseId = endpointResponse2.DeserializedResponse.DeviceId;
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
        var endpointResponse = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);

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
            await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);

        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task RegisterNewDevice_WrongParameters_InvalidName()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "        \n \t";
        var payload = new DeviceAddParameters()
        {
            OwnerId = owner.Id,
            Name = name
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Name", errorMessage);
    }


    [Fact]
    public async Task AttachRoleToDevice_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.True(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }
    
    [Fact]
    public async Task AttachRoleToDevice_AlreadyAttached_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        var secondEndpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);

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
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = new Guid(), // this should be all zeroes 
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
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
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
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
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = new Guid(),
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyDevice_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task AttachRoleToDevice_WrongRole_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);

        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = Guid.NewGuid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
    }
    
    [Fact]
    public async Task AttachRoleToDevice_EmptyRole_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);

        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
    }

    [Fact]
    public async Task AttachRoleToDevice_DeviceDoesntBelongToOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner2);
        var role = await GetRoleAsync(owner);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
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
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner2);
        
        var payload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleId), errorMessage);
        Assert.False(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, new CancellationToken()));
    }


    [Fact]
    public async Task GetAllRolesForDevice_DeviceHas1Role_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);

        var attachPayload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        
        // Attach Role to Device
        var attachResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, attachPayload);
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
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
    public async Task GetAllRolesForDevice_EmptyResponse_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);

        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };
        
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
                HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var roles = endpointResponse.DeserializedResponse.Roles.ToArray();
        Assert.NotNull(roles);
        Assert.True(roles.Length == 0);
    }
    
    [Fact]
    public async Task GetAllRolesForDevice_DeviceHasMultipleRoles_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role1 = await GetRoleAsync(owner);
        
        
        var attachPayload1 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role1.Id
        };
        
        var attachResponse1 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, attachPayload1);
        attachResponse1.HttpResponse.EnsureSuccessStatusCode();
        
        
        var role2 = await GetRoleAsync(owner);
        
        var attachPayload2 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role2.Id
        };
        
        var attachResponse2 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post, attachPayload2);
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
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
        var owner = await GetOwnerAsync();
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
        var owner = await GetOwnerAsync();
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
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
        var owner1 = await GetOwnerAsync();
        
        var owner2 = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner2);
        
        var payload = new GetAllRolesForDeviceParameters()
        {
            OwnerId = owner1.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllRolesForDeviceParameters, GetAllRolesForDeviceResult>(GetAllRolesForDeviceUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }
    
    [Fact]
    public async Task GetOpeningKeys_OneKey_OneRole_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        // Attach Device to Role
        var attachDeviceRolePayload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        var attachDeviceRoleResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post,
                attachDeviceRolePayload);
        attachDeviceRoleResponse.HttpResponse.EnsureSuccessStatusCode();
        
        // Attach Role to Key - issue a new key
        var attachRoleKeyPayload = new IssueNewKeyParameters()
        {
            Name = "test klic",
            RoleIds = new[] { role.Id }
        };
        var attachRoleKeyResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNeKeyUri, HttpMethod.Post,
                attachRoleKeyPayload);
        attachRoleKeyResponse.HttpResponse.EnsureSuccessStatusCode();
        // Get the new key
        var keyId = attachRoleKeyResponse.DeserializedResponse.KeyId;
        var key = await keyService.GetByIdAsync(keyId, new CancellationToken());
        
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.NotNull(endpointResponse.DeserializedResponse.OpeningKeys);
        var openingKeys = endpointResponse.DeserializedResponse.OpeningKeys.ToArray();
        Assert.True(openingKeys.Length == 1);
        Assert.True(openingKeys[0].KeyId == key.Id);
        Assert.Contains(key.Pass, openingKeys[0].Token);
        Assert.True(key.Name == "test klic");
    }
    
    [Fact]
    public async Task GetOpeningKeys_OneKey_OneRole_RUNTWICE_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role = await GetRoleAsync(owner);
        
        // Attach Device to Role
        var attachDeviceRolePayload = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role.Id
        };
        var attachDeviceRoleResponse =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post,
                attachDeviceRolePayload);
        attachDeviceRoleResponse.HttpResponse.EnsureSuccessStatusCode();
        
        // Attach Role to Key - issue a new key
        var attachRoleKeyPayload = new IssueNewKeyParameters()
        {
            Name = "test klic",
            RoleIds = new[] { role.Id }
        };
        var attachRoleKeyResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNeKeyUri, HttpMethod.Post,
                attachRoleKeyPayload);
        attachRoleKeyResponse.HttpResponse.EnsureSuccessStatusCode();
        // Get the new key
        var keyId = attachRoleKeyResponse.DeserializedResponse.KeyId;
        var key = await keyService.GetByIdAsync(keyId, new CancellationToken());
        
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse1 =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);

        var endpointResponse2 =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        endpointResponse1.HttpResponse.EnsureSuccessStatusCode();
        endpointResponse2.HttpResponse.EnsureSuccessStatusCode();
        Assert.NotNull(endpointResponse2.DeserializedResponse.OpeningKeys);
        var openingKeys = endpointResponse2.DeserializedResponse.OpeningKeys.ToArray();
        Assert.True(openingKeys.Length == 1);
        Assert.True(openingKeys[0].KeyId == key.Id);
        Assert.Contains(key.Pass, openingKeys[0].Token);
        Assert.True(key.Name == "test klic");
    }

    [Fact]
    public async Task GetOpeningKeys_TwoKeys_MultipleRoles_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);
        var role1 = await GetRoleAsync(owner);
        var role2 = await GetRoleAsync(owner);
        
        // Attach Device to Role1 and Role2
        var attachDeviceRolePayload1 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role1.Id
        };
        var attachDeviceRoleResponse1 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post,
                attachDeviceRolePayload1);
        attachDeviceRoleResponse1.HttpResponse.EnsureSuccessStatusCode();
        
        var attachDeviceRolePayload2 = new AttachRoleToDeviceParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id,
            RoleId = role2.Id
        };
        var attachDeviceRoleResponse2 =
            await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri, HttpMethod.Post,
                attachDeviceRolePayload2);
        attachDeviceRoleResponse2.HttpResponse.EnsureSuccessStatusCode();
        
        // Attach Key1 to Role1
        var attachRoleKeyPayload1 = new IssueNewKeyParameters()
        {
            Name = "klic1",
            RoleIds = new[] { role1.Id }
        };
        var attachRoleKeyResponse1 =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNeKeyUri, HttpMethod.Post,
                attachRoleKeyPayload1);
        attachRoleKeyResponse1.HttpResponse.EnsureSuccessStatusCode();
        // Get the new key1
        var keyId = attachRoleKeyResponse1.DeserializedResponse.KeyId;
        var key1 = await keyService.GetByIdAsync(keyId, new CancellationToken());
        
        // Attach Key2 to Role1
        var attachRoleKeyPayload2 = new IssueNewKeyParameters()
        {
            Name = "klic2",
            RoleIds = new[] { role1.Id }
        };
        var attachRoleKeyResponse2 =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNeKeyUri, HttpMethod.Post,
                attachRoleKeyPayload2);
        attachRoleKeyResponse2.HttpResponse.EnsureSuccessStatusCode();
        // Get the new key
        keyId = attachRoleKeyResponse2.DeserializedResponse.KeyId;
        var key2 = await keyService.GetByIdAsync(keyId, new CancellationToken());
        
        // Attach Key3 to Role2
        var attachRoleKeyPayload3 = new IssueNewKeyParameters()
        {
            Name = "klic3",
            RoleIds = new[] { role2.Id }
        };
        var attachRoleKeyResponse3 =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNeKeyUri, HttpMethod.Post,
                attachRoleKeyPayload3);
        attachRoleKeyResponse3.HttpResponse.EnsureSuccessStatusCode();
        // Get the new key
        keyId = attachRoleKeyResponse3.DeserializedResponse.KeyId;
        var key3 = await keyService.GetByIdAsync(keyId, new CancellationToken());

        var Keys = new[] { key1, key2, key3 };
        
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };

        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.NotNull(endpointResponse.DeserializedResponse.OpeningKeys);
        var openingKeys = endpointResponse.DeserializedResponse.OpeningKeys.ToArray();
        Assert.True(openingKeys.Length == 3);

        foreach (var key in Keys)
        {
            var found = false;
            foreach (var resultKey in openingKeys)
            {
                if (key.Id == resultKey.KeyId && resultKey.Token.Contains(key.Pass))
                    found = true;
            }
            Assert.True(found);
        }
    }

    [Fact]
    public async Task GetOpeningKeys_EmptyOwner_Fail()
    {
        // Arrange
        var payload = new GetOpeningKeysParameters()
        {
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task GetOpeningKeys_WrongOwner_Fail()
    {
        // Arrange
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = new Guid(),
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
    }

    [Fact]
    public async Task GetOpeningKeys_EmptyDevice_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task GetOpeningKeys_WrongDevice_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = new Guid()
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task GetOpeningKeys_DeviceDoesntBelongToOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        var owner2 = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner2);
        
        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.OwnerId), errorMessage);
        Assert.Contains(nameof(payload.DeviceId), errorMessage);
    }

    [Fact]
    public async Task GetOpeningKeys_EmptyResponse_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var device = await GetDeviceAsync(owner);

        var payload = new GetOpeningKeysParameters()
        {
            OwnerId = owner.Id,
            DeviceId = device.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
                HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.NotNull(endpointResponse.DeserializedResponse.OpeningKeys);
        var openingKeys = endpointResponse.DeserializedResponse.OpeningKeys.ToArray();
        Assert.True(openingKeys.Length == 0);
    }
}
