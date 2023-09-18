using System.Net;
using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using Core.Entities.Devices;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class KeysApiTests : ApiTestsSetup
{
    [Fact]
    public async Task IssueNewKey_OneRole_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var responseKeyId = endpointResponse.DeserializedResponse.KeyId;
        var keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.First().Id == role.Id);
    }

    [Fact]
    public async Task IssueNewKey_MultipleRoles_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        var role2 = await GetRoleAsync(owner);
        const string name = "new key name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id , role2.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var responseKeyId = endpointResponse.DeserializedResponse.KeyId;
        var keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.Count == 2);
        Assert.True(keyFromDb.Roles[0].Id == role.Id || keyFromDb.Roles[0].Id == role2.Id);
        Assert.True(keyFromDb.Roles[1].Id == role.Id || keyFromDb.Roles[1].Id == role2.Id);
    }

    [Fact]
    public async Task IssueNewKey_NameWhitespace_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "    \n  \t";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.Name), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_EmptyName_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);

        var payload = new IssueNewKeyParameters()
        {
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.Name), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_NullRoles_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "name test";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = null
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleIds), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_WrongRoles_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "name test";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { new Guid() }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleIds), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_RolesNotExisting_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "name test";

        var payload = new IssueNewKeyParameters()
        {
            Name = name
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.RoleIds), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_RepeatApiCall_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var endpointResponse2 =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        endpointResponse2.HttpResponse.EnsureSuccessStatusCode();

        
        // Assert
        var responseKeyId = endpointResponse2.DeserializedResponse.KeyId;
        var keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.First().Id == role.Id);
        
        responseKeyId = endpointResponse.DeserializedResponse.KeyId;
        keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.First().Id == role.Id);
    }
    
    [Fact]
    public async Task IssueNewKey_RoleWrongOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();

        var owner2 = await GetOwnerAsync();
        var role = await GetRoleAsync(owner2);
        
        var name = "test name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Id", errorMessage);
        Assert.Contains(nameof(payload.RoleIds), errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_EmptyOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        var name = "test name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, "", HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.Unauthorized);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Equal("", errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_WrongOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        var name = "test name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id }
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner2.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Id", errorMessage);
    }

    [Fact]
    public async Task IssueNewKey_EndValidityInFuture_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";
        var validityEnd = DateTime.UtcNow + TimeSpan.FromMinutes(10);

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id },
            ValidityEndUtc = validityEnd
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var responseKeyId = endpointResponse.DeserializedResponse.KeyId;
        var keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.First().Id == role.Id);
        Assert.True(keyFromDb.ValidityEndUtc == validityEnd);
    }

    [Fact]
    public async Task IssueNewKey_EndValidityInPast_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id },
            ValidityEndUtc = DateTime.UtcNow - TimeSpan.FromMinutes(10)
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Key validity cannot end in the past.", errorMessage);

    }

    [Fact]
    public async Task IssueNewKey_StartEndValidityRight_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";
        var validityEnd = DateTime.UtcNow + TimeSpan.FromMinutes(10);
        var validityStart = DateTime.UtcNow - TimeSpan.FromMinutes(10);

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id },
            ValidityStartUtc = validityStart,
            ValidityEndUtc = validityEnd
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var responseKeyId = endpointResponse.DeserializedResponse.KeyId;
        var keyFromDb = await keyService.GetByIdAsync(responseKeyId, new CancellationToken());
        
        Assert.NotNull(keyFromDb);
        Assert.True(keyFromDb.Name == name);
        Assert.NotNull(keyFromDb.Roles);
        Assert.True(keyFromDb.Roles.First().Id == role.Id);
        Assert.True(keyFromDb.ValidityStartUtc == validityStart);
        Assert.True(keyFromDb.ValidityEndUtc == validityEnd);
    }
    
    [Fact]
    public async Task IssueNewKey_StartEndValidityWrong_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var role = await GetRoleAsync(owner);
        const string name = "new key name";
        var validityStart = DateTime.UtcNow + TimeSpan.FromMinutes(10);
        var validityEnd = DateTime.UtcNow + TimeSpan.FromMinutes(5);

        var payload = new IssueNewKeyParameters()
        {
            Name = name,
            RoleIds = new[] { role.Id },
            ValidityStartUtc = validityStart,
            ValidityEndUtc = validityEnd
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Validity cannot end before start.", errorMessage);
    }


    [Fact]
    public async Task GetAllKeys_All_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key1 = await GetKeyAsync(owner);
        var validityEnd = DateTime.UtcNow + TimeSpan.FromMinutes(10);
        var key2 = await GetKeyAsync(owner, DateTime.UtcNow, validityEnd);

        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = false
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var resultKeys = endpointResponse.DeserializedResponse.Keys.ToArray();
        Assert.True(resultKeys.Length == 2);
        foreach (var key in resultKeys)
        {
            Assert.True(key.Name == key1.Name); // key2 has same name

            if (key.Id == key1.Id)
            {
                Assert.True(key.Validity.EndUtc == null);
            }
            else if (key.Id == key2.Id)
            {
                Assert.True(key.Validity.EndUtc == validityEnd);
            }
            else Assert.Fail("Key ID different from prepared test keys");
        }
    }

    [Fact]
    public async Task GetAllKeys_OnlyValid_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key1 = await GetKeyAsync(owner);
        
        // this key shouldnt be in the results
        var validityStart = DateTime.UtcNow - TimeSpan.FromMinutes(2);
        var validityEnd = DateTime.UtcNow - TimeSpan.FromMinutes(1);
        var key2 = await GetKeyAsync(owner, validityStart, validityEnd);

        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = true
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var resultKeys = endpointResponse.DeserializedResponse.Keys.ToArray();
        Assert.True(resultKeys.Length == 1);

        var key = resultKeys.First();
        Assert.True(key.Name == key1.Name);
        Assert.True(key.Id == key1.Id);
        Assert.True(key.Validity.EndUtc == null);
    }

    [Fact]
    public async Task GetAllKeys_NoKeysInDb_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = true
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Keys);
    }
    
    [Fact]
    public async Task GetAllKeys_EmptyParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        var payload = new GetAllKeysParameters();
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Keys);
    }

    [Fact]
    public async Task GetAllKeys_NullParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Payload cannot be null", errorMessage);
    }

    [Fact]
    public async Task GetAllKeys_Unauthorized_Fail()
    {
        // Arrange
        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = false
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            "", HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.Unauthorized);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Equal("", errorMessage);
    }

    [Fact]
    public async Task GetAllKeys_AllKeysInvalid_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        
        var validityStart = DateTime.UtcNow - TimeSpan.FromMinutes(2);
        var validityEnd = DateTime.UtcNow - TimeSpan.FromMinutes(1);
        
        // keys shouldnt be in the results
        var key1 = await GetKeyAsync(owner, validityStart, validityEnd);
        var key2 = await GetKeyAsync(owner, validityStart, validityEnd);
        var key3 = await GetKeyAsync(owner2);

        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = true
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Keys);
    }

    [Fact]
    public async Task GetAllKeys_WrongOwner_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        
        var key1 = await GetKeyAsync(owner2);
        var key2 = await GetKeyAsync(owner2);

        var payload = new GetAllKeysParameters()
        {
            GetCurrentlyValidOnly = false
        };
        
        // Act
        var endpointResponse = await CallEndpoint<GetAllKeysParameters, GetAllKeysResult>(GetAllKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Keys);
    }

    [Fact]
    public async Task DeleteKey_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);

        var payload = new KeyDeleteParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Equal("", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeleteKey_NoKeyToDelete_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new KeyDeleteParameters();

        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Equal("", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeleteKey_WrongOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        var key = await GetKeyAsync(owner2);

        var payload = new KeyDeleteParameters()
        {
            KeyId = key.Id
        };

        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        Assert.Contains(nameof(payload.KeyId), await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeleteKey_NullParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();

        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Payload cannot be null", errorMessage);
    }

    [Fact]
    public async Task DeleteKey_RunTwice_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);

        var payload = new KeyDeleteParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        var endpointResponse2 = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, payload);
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        endpointResponse2.HttpResponse.EnsureSuccessStatusCode();
        
        Assert.Equal("", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
        Assert.Equal("", await endpointResponse2.HttpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeleteKey_Unauthorized_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);

        var payload = new KeyDeleteParameters()
        {
            KeyId = key.Id
        };

        // Act
        var endpointResponse = await CallEndpoint<KeyDeleteParameters, KeyDeleteResult>(DeleteKeysUri,
            "", HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.Unauthorized);
        Assert.Equal("", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAllUnlockingDevices_OneDevice_OneRole_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        var key = await GetKeyAsync(owner);
        var role = await GetRoleAsync(owner);
        var device = await GetDeviceAsync(owner);

        // Attach Key <-> Role
        var attachKeyToRolePayload = new AttachRoleToKeyParameters()
        {
            KeyId = key.Id,
            RoleId = role.Id
        };
        var attachRoleToKeyCallResponse = await CallEndpoint<AttachRoleToKeyParameters, Task>(AttachRoleToKeyUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachKeyToRolePayload);
        attachRoleToKeyCallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        // Attach Role <-> Device
        var attachRoleToDevicePayload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device.Id,
            RoleId = role.Id
        };
        var attachRoleToDeviceCallResponse = await CallEndpoint<AttachRoleToDeviceParameters, Task>(
            AttachRoleToDeviceUri, owner.Accounts.First().Email, HttpMethod.Post, attachRoleToDevicePayload);
        attachRoleToDeviceCallResponse.HttpResponse.EnsureSuccessStatusCode();

        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var deviceResults = endpointResponse.DeserializedResponse.Devices.ToArray();
        Assert.True(deviceResults.Length == 1);
        Assert.True(deviceResults.First().Id == device.Id);
        Assert.True(deviceResults.First().Name == "test device");
    }

    [Fact]
    public async Task GetAllUnlockingDevices_MultipleDevices_MultipleRoles_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();

        var key = await GetKeyAsync(owner);
        
        var role1 = await GetRoleAsync(owner);
        var role2 = await GetRoleAsync(owner);

        var device1 = await GetDeviceAsync(owner);
        var device2 = await GetDeviceAsync(owner);
        var device3 = await GetDeviceAsync(owner);
        
        var attachRole1ToKeyPayload = new AttachRoleToKeyParameters()
        {
            KeyId = key.Id,
            RoleId = role1.Id
        };
        var attachRole2ToKeyPayload = new AttachRoleToKeyParameters()
        {
            KeyId = key.Id,
            RoleId = role2.Id
        };

        var attachRole1ToDevice1Payload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device1.Id,
            RoleId = role1.Id
        };
        var attachRole1ToDevice2Payload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device2.Id,
            RoleId = role1.Id
        };
        var attachRole2ToDevice3Payload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device3.Id,
            RoleId = role2.Id
        };

        var attachRole1ToKeyCallResponse = await CallEndpoint<AttachRoleToKeyParameters, Task>(AttachRoleToKeyUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachRole1ToKeyPayload);
        attachRole1ToKeyCallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var attachRole2ToKeyCallResponse = await CallEndpoint<AttachRoleToKeyParameters, Task>(AttachRoleToKeyUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachRole2ToKeyPayload);
        attachRole2ToKeyCallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var attachRole1ToDevice1CallResponse = await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachRole1ToDevice1Payload);
        attachRole1ToDevice1CallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var attachRole1ToDevice2CallResponse = await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachRole1ToDevice2Payload);
        attachRole1ToDevice2CallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        var attachRole2ToDevice3CallResponse = await CallEndpoint<AttachRoleToDeviceParameters, Task>(AttachRoleToDeviceUri,
            owner.Accounts.First().Email, HttpMethod.Post, attachRole2ToDevice3Payload);
        attachRole2ToDevice3CallResponse.HttpResponse.EnsureSuccessStatusCode();

        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        var resultDevices = endpointResponse.DeserializedResponse.Devices.ToArray();
        Assert.True(resultDevices.Length == 3);
        foreach (var resultDevice in resultDevices)
        {
            Assert.True(resultDevice.Id == device1.Id || resultDevice.Id == device2.Id ||
                        resultDevice.Id == device3.Id);
            Assert.True(resultDevice.Type == DeviceType.Lock.ToString());
            Assert.True(resultDevice.Name == "test device");
        }
    }

    [Fact]
    public async Task GetAllUnlockingDevices_NoDevice_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);

        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Devices);
    }

    [Fact]
    public async Task GetAllUnlockingDevices_NullParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Payload cannot be null", errorMessage);

    }

    [Fact]
    public async Task GetAllUnlockingDevices_EmptyParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new GetAllUnlockingDevicesParameters();
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.KeyId), errorMessage);
    }

    [Fact]
    public async Task GetAllUnlockingDevices_Unauthorized_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);
        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, "", HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.Unauthorized);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Equal("", errorMessage);
    }

    [Fact]
    public async Task GetAllUnlockingDevices_WrongOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var owner2 = await GetOwnerAsync();
        
        var key = await GetKeyAsync(owner2);
        var role = await GetRoleAsync(owner2);
        var device = await GetDeviceAsync(owner2);

        // Attach Key <-> Role
        var attachKeyToRolePayload = new AttachRoleToKeyParameters()
        {
            KeyId = key.Id,
            RoleId = role.Id
        };
        var attachRoleToKeyCallResponse = await CallEndpoint<AttachRoleToKeyParameters, Task>(AttachRoleToKeyUri,
            owner2.Accounts.First().Email, HttpMethod.Post, attachKeyToRolePayload);
        attachRoleToKeyCallResponse.HttpResponse.EnsureSuccessStatusCode();
        
        // Attach Role <-> Device
        var attachRoleToDevicePayload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device.Id,
            RoleId = role.Id
        };
        var attachRoleToDeviceCallResponse = await CallEndpoint<AttachRoleToDeviceParameters, Task>(
            AttachRoleToDeviceUri, owner2.Accounts.First().Email, HttpMethod.Post, attachRoleToDevicePayload);
        attachRoleToDeviceCallResponse.HttpResponse.EnsureSuccessStatusCode();

        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.KeyId), errorMessage);
    }

    [Fact]
    public async Task GetAllUnlockingDevices_NoDeviceInDb_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var key = await GetKeyAsync(owner);

        var payload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        Assert.Empty(endpointResponse.DeserializedResponse.Devices);
    }
}
