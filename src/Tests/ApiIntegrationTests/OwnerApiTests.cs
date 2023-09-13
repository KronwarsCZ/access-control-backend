using ApiShared.Dto.Owners;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class OwnerApiTests : ApiTestsSetup
{
    [Fact]
    public async Task RegisterNewOwner_Succeeds()
    {
        var password = "myGlorIouSpASSword1235";
        var email = "martin@martin.com";
        var accountName = "martinovo account name";

        var registerNewOwnerUri = new Uri("api/v1/registerNewOwner", UriKind.Relative);
        var payload = new RegisterNewOwnerParameters
        {
            Email = email,
            Name = accountName,
            Password = password
        };
        var endpointResponse = await CallEndpoint<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(registerNewOwnerUri, HttpMethod.Post, payload);
        endpointResponse.HttpResponse.EnsureSuccessStatusCode();

        var ownerIdFromResponse = endpointResponse.DeserializedResponse.OwnerId;

        var ownerReturnedFromDb = await ownerService.GetByIdAsync(ownerIdFromResponse, cancellationToken);
            
        Assert.NotNull(ownerReturnedFromDb);
        Assert.Equal(ownerIdFromResponse, ownerReturnedFromDb.Id);
    }
}
