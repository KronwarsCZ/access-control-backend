using System.Net;
using ApiShared.Dto.Roles;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class RoleApiTests : ApiTestsSetup
{
    [Fact]
    public async Task AddNewRole_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "test role";
        var payload = new RoleAddParameters()
        {
            Name = name
        };

        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();
        
        Assert.NotNull(endpointResponse.DeserializedResponse);
        Assert.True(endpointResponse.DeserializedResponse.RoleId != Guid.Empty);
        var roleFromDb = await roleService.GetByIdAsync(endpointResponse.DeserializedResponse.RoleId, new CancellationToken());
        Assert.True(roleFromDb.Name == name);
        Assert.True(roleFromDb.OwnerId == owner.Id);
    }

    [Fact]
    public async Task AddNewRole_EmptyOwner_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new RoleAddParameters()
        {
            Name = "test role"
        };
        
        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, "", HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.Unauthorized);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Equal("", errorMessage);
    }

    [Fact]
    public async Task AddNewRole_WhitespaceName_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "   \n   \t  ";
        var payload = new RoleAddParameters()
        {
            Name = name
        };
        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.Name), errorMessage);
    }

    [Fact]
    public async Task AddNewRole_EmptyName_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new RoleAddParameters();
        
        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains(nameof(payload.Name), errorMessage);
    }

    [Fact]
    public async Task AddNewRole_SecondRun_Success()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        const string name = "test role";
        var payload = new RoleAddParameters()
        {
            Name = name
        };

        // Act
        var endpointResponse1 =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post, payload);
        var endpointResponse2 =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post, payload);

        // Assert
        endpointResponse1.HttpResponse.EnsureSuccessStatusCode();
        
        Assert.NotNull(endpointResponse1.DeserializedResponse);
        Assert.True(endpointResponse1.DeserializedResponse.RoleId != Guid.Empty);
        var roleFromDb = await roleService.GetByIdAsync(endpointResponse1.DeserializedResponse.RoleId, new CancellationToken());
        Assert.True(roleFromDb.Name == name);
        Assert.True(roleFromDb.OwnerId == owner.Id);
        
        endpointResponse2.HttpResponse.EnsureSuccessStatusCode();
        
        Assert.NotNull(endpointResponse2.DeserializedResponse);
        Assert.True(endpointResponse2.DeserializedResponse.RoleId != Guid.Empty);
        roleFromDb = await roleService.GetByIdAsync(endpointResponse2.DeserializedResponse.RoleId, new CancellationToken());
        Assert.True(roleFromDb.Name == name);
        Assert.True(roleFromDb.OwnerId == owner.Id);
    }
    
    [Fact]
    public async Task AddNewRole_NullParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        
        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email, HttpMethod.Post);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        Assert.Contains("Payload cannot be null", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task AddNewRole_EmptyParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new RoleAddParameters();
        
        // Act
        var endpointResponse =
            await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri, owner.Accounts.First().Email,
                HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
    }
}
