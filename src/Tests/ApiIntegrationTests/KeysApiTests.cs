using System.Net;
using ApiShared.Dto.Keys;
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
}
