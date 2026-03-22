using System.Net;
using ApiShared.Dto.Owners;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class OwnersApiTests : ApiTestsSetup
{
    [Fact]
    public async Task RegisterNewOwner_Succeeds()
    {
        var password = "myGlorIouSpASSword1235-";
        var email = "martin@martin.com";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();

        var ownerIdFromResponse = endpointResponse.DeserializedResponse.OwnerId;

        var ownerReturnedFromDb = await ownerService.GetByIdAsync(ownerIdFromResponse, cancellationToken);
            
        Assert.NotNull(ownerReturnedFromDb);
        Assert.Equal(ownerIdFromResponse, ownerReturnedFromDb.Id);
    }

    [Fact]
    public async Task RegisterNewOwner_RunTwice_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var email = "petr@budai.com";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();

        var ownerIdFromResponse = endpointResponse.DeserializedResponse.OwnerId;

        var ownerReturnedFromDb = await ownerService.GetByIdAsync(ownerIdFromResponse, cancellationToken);

        var endpointResponse2 =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, payload);

        // Assert
        Assert.NotNull(ownerReturnedFromDb);
        Assert.Equal(ownerIdFromResponse, ownerReturnedFromDb.Id);
        
        Assert.True(endpointResponse2.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse2.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("already exists", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_EmptyName_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var email = "martin@martin.com";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains($"{nameof(payload.Name)}", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_WhitespaceName_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var email = "martin@martin.com";
        var accountName = "    \t   \n  ";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains($"{nameof(payload.Name)}", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_EmptyEmail_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains($"{nameof(payload.Email)}", errorMessage);

    }

    [Fact]
    public async Task RegisterNewOwner_WhitespaceEmail_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var email = "    \n   \t";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("email", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_BadFormatEmail_Fail()
    {
        // Arrange
        var password = "myGlorIouSpASSword1235-";
        var email = "aaa.com";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("format", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_EmptyPassword_Fail()
    {
        // Arrange
        var email = "martin@martin.com";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains($"{nameof(payload.Password)}", errorMessage);
    }
    
    [Fact]
    public async Task RegisterNewOwner_WhitespacePassword_Fail()
    {
        // Arrange
        var email = "martin@martin.com";
        var password = "  \n   \t  ";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Password = password,
            Name = accountName,
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("Password", errorMessage);
    }

    [Fact]
    public async Task RegisterNewOwner_PasswordCriteriaUnmet_Fail()
    {
        // Arrange
        var password = "aa";
        var email = "martin@martin.com";
        var accountName = "martinovo account name";

        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        var errorMessage = await endpointResponse.HttpResponse.Content.ReadAsStringAsync();
        Assert.Contains("password", errorMessage);
    }
    
    [Fact]
    public async Task RegisterNewOwner_NullParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
        Assert.Contains("Payload cannot be null", await endpointResponse.HttpResponse.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task RegisterNewOwner_EmptyParams_Fail()
    {
        // Arrange
        var owner = await GetOwnerAsync();
        var payload = new RegisterNewOwnerParameters();
        
        // Act
        var endpointResponse =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, payload);
        
        // Assert
        Assert.True(endpointResponse.HttpResponse.StatusCode == HttpStatusCode.BadRequest);
    }
}
