using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using ApiShared.Dto.Owners;
using ApiShared.Dto.Roles;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public class GigaTest : ApiTestsSetup
{
    [Fact]
    public async Task Gigatest_Success()
    {
        var ct = new CancellationToken();
        
        // Register
        var registerPayload = new RegisterNewOwnerParameters()
        {
            Email = "petr@budai.cz",
            Name = "Petruv ucet",
            Password = TestPass
        };
        var registerCall =
            await CallEndpointWithoutAuthorization<RegisterNewOwnerParameters, RegisterNewOwnerResponse>(
                RegisterNewOwnerUri, HttpMethod.Post, registerPayload);
        registerCall.HttpResponse.EnsureSuccessStatusCode();

        var owner = await ownerService.GetByIdAsync(registerCall.DeserializedResponse.OwnerId, ct);
        Assert.True(owner.Name == "Petruv ucet");
        
        // Add new device
        var addDevicePayload = new DeviceAddParameters()
        {
            Name = "novej device"
        };
        var registerDeviceCall = await CallEndpoint<DeviceAddParameters, DeviceAddResult>(RegisterNewDeviceUri,
            owner.Accounts.First().Email, HttpMethod.Post, addDevicePayload);
        registerDeviceCall.HttpResponse.EnsureSuccessStatusCode();

        var device = await deviceService.GetByIdAsync(registerDeviceCall.DeserializedResponse.DeviceId, ct);
        Assert.True(device.Name == "novej device");
        
        
        // Add new role
        var addRolePayload = new RoleAddParameters()
        {
            Name = "nova role xD"
        };
        var addRoleCall = await CallEndpoint<RoleAddParameters, RoleAddResult>(AddNewRoleUri,
            owner.Accounts.First().Email, HttpMethod.Post, addRolePayload);
        addRoleCall.HttpResponse.EnsureSuccessStatusCode();

        var role = await roleService.GetByIdAsync(addRoleCall.DeserializedResponse.RoleId, ct);
        Assert.True(role.Name == "nova role xD");
        
        // Issue new key
        var issueKeyPayload = new IssueNewKeyParameters()
        {
            Name = "klicek",
            KeyUid = "AABBCCDD",
            RoleIds = new[] { role.Id }
        };
        var issueKeyCall = await CallEndpoint<IssueNewKeyParameters, IssueNewKeyResult>(IssueNewKeyUri,
            owner.Accounts.First().Email, HttpMethod.Post, issueKeyPayload);
        issueKeyCall.HttpResponse.EnsureSuccessStatusCode();
        
        var key = await keyService.GetByIdAsync(issueKeyCall.DeserializedResponse.KeyId, ct);
        Assert.True(key.Name == "klicek");
        
        // Attach Device To Role
        // (key and role already attached)
        var attachRoleToDevicePayload = new AttachRoleToDeviceParameters()
        {
            DeviceId = device.Id,
            RoleId = role.Id
        };
        var attachRoleToDeviceCall =
            await CallEndpoint<AttachRoleToDeviceParameters, AttachRoleToDeviceResult>(AttachRoleToDeviceUri,
                owner.Accounts.First().Email, HttpMethod.Post, attachRoleToDevicePayload);
        attachRoleToDeviceCall.HttpResponse.EnsureSuccessStatusCode();
        Assert.True(await deviceRoleService.IsDeviceRoleConnectionExisting(device.Id, role.Id, ct));
        
        // GetOpeningKeys
        var getOpeningKeysPayload = new GetOpeningKeysParameters()
        {
            DeviceId = device.Id
        };
        var getOpeningKeysCall = await CallEndpoint<GetOpeningKeysParameters, GetOpeningKeysResult>(GetOpeningKeysUri,
            owner.Accounts.First().Email, HttpMethod.Post, getOpeningKeysPayload);
        getOpeningKeysCall.HttpResponse.EnsureSuccessStatusCode();
        
        var openingKeysResult = getOpeningKeysCall.DeserializedResponse.OpeningKeys.ToArray();
        Assert.True(openingKeysResult.Length == 1);
        Assert.True(openingKeysResult.First().KeyId == key.Id);
        
        // GetAllUnlockingDevices
        var getUnlockableDevicesPayload = new GetAllUnlockingDevicesParameters()
        {
            KeyId = key.Id
        };
        var getUnlockableDevicesCall =
            await CallEndpoint<GetAllUnlockingDevicesParameters, GetAllUnlockingDevicesResult>(
                GetAllUnlockingDevicesUri, owner.Accounts.First().Email, HttpMethod.Post, getUnlockableDevicesPayload);
        getUnlockableDevicesCall.HttpResponse.EnsureSuccessStatusCode();

        var unlockableDevicesResult = getUnlockableDevicesCall.DeserializedResponse.Devices.ToArray();
        Assert.True(unlockableDevicesResult.Length == 1);
        Assert.True(unlockableDevicesResult.First().Id == device.Id);
    }
}
