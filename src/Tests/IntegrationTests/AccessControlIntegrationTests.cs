using AccessControl.Web;
using Application.IUnitOfWork;
using Application.Services.DeviceRole;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.Services.Roles;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Core.Entities.Owners;
using Core.Entities.Roles;
using Framework.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class AccessControlIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    protected WebApplicationFactory<Program> webApplicationFactory;
    protected IServiceScope scope;
    private IUnitOfWork unitOfWork;
    protected HttpClient _httpClient;
    protected CancellationToken cancellationToken => new CancellationToken();

    public AccessControlIntegrationTests()
    {
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        webApplicationFactory = new WebApplicationFactory<Program>();
        _httpClient = webApplicationFactory.CreateDefaultClient();
        scope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
    
    protected async Task<Owner> GetOwnerAsync()
    {
        var owner = Owner.Create("sample test owner");
        await ownerService.RegisterNewOwnerAsync(owner, new CancellationToken());
        await ownerService.SaveChangesAsync(new CancellationToken());
        return await ownerService.GetByIdAsync(owner.Id, new CancellationToken());
    }

    protected async Task<Device> GetDeviceAsync(Owner owner)
    {
        var device = Device.Create(owner, "test device", DeviceType.Lock);
        await deviceService.RegisterNewDeviceAsync(device, new CancellationToken());
        await deviceService.SaveChangesAsync(new CancellationToken());
        return await deviceService.GetByIdAsync(device.Id, new CancellationToken());
    }

    protected async Task<Role> GetRoleAsync(Owner owner)
    {
        var role = Role.Create(owner, "test role");
        await roleService.AddAsync(role.ToEnumerable(), new CancellationToken());
        await roleService.SaveChangesAsync(new CancellationToken());
        return await roleService.GetByIdAsync(role.Id, new CancellationToken());
    }

    protected async Task<Key> GetKeyAsync(Owner owner)
    {
        var key = Key.Create(owner, "test key");
        var ct = new CancellationToken();
        await keyService.AddAsync(key.ToEnumerable(), ct);
        await keyService.SaveChangesAsync(ct);
        return await keyService.GetByIdAsync(key.Id, ct);
    }

    // Services:
    protected IDeviceService deviceService => scope.ServiceProvider.GetRequiredService<IDeviceService>();
    protected IKeyService keyService => scope.ServiceProvider.GetRequiredService<IKeyService>();
    protected IOwnerService ownerService => scope.ServiceProvider.GetRequiredService<IOwnerService>();
    protected IRoleService roleService => scope.ServiceProvider.GetRequiredService<IRoleService>();
    protected IDeviceRoleService deviceRoleService => scope.ServiceProvider.GetRequiredService<IDeviceRoleService>();
    
    // Uris:
    protected readonly Uri GetOpeningKeysUri = new Uri("api/v1/devices/getOpeningKeys", UriKind.Relative);
    protected readonly Uri IssueNewKeyUri = new Uri("api/v1/keys/issueNewKey", UriKind.Relative);
    protected readonly Uri GetAllRolesForDeviceUri = new Uri("api/v1/devices/getAllRolesForDevice", UriKind.Relative);
    protected readonly Uri AttachRoleToDeviceUri = new("api/v1/devices/attachRoleToDevice", UriKind.Relative);
    protected readonly Uri RegisterNewDeviceUri = new("api/v1/devices/registerNewDevice", UriKind.Relative);
    protected readonly Uri AddNewRole = new Uri("api/v1/roles/addNewRole", UriKind.Relative);
}
