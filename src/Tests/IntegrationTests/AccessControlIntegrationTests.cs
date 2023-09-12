using AccessControl.Web;
using Application.IUnitOfWork;
using Application.Services;
using Application.Services.Devices;
using Application.Services.JointService;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.Services.Roles;
using Core.Entities.Devices;
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
    
    protected async Task<Owner> GetOwner()
    {
        var owner = Owner.Create("sample test owner");
        await ownerService.RegisterNewOwnerAsync(owner, new CancellationToken());
        await ownerService.SaveChangesAsync(new CancellationToken());
        return await ownerService.GetByIdAsync(owner.Id, new CancellationToken());
    }

    protected async Task<Device> GetDevice(Owner owner)
    {
        var device = Device.Create(owner, "test device", DeviceType.Lock);
        await deviceService.RegisterNewDeviceAsync(device, new CancellationToken());
        await deviceService.SaveChangesAsync(new CancellationToken());
        return await deviceService.GetByIdAsync(device.Id, new CancellationToken());
    }

    protected async Task<Role> GetRole(Owner owner)
    {
        var role = Role.Create(owner, "test role");
        await roleService.AddAsync(role.ToEnumerable(), new CancellationToken());
        await roleService.SaveChangesAsync(new CancellationToken());
        return await roleService.GetByIdAsync(role.Id, new CancellationToken());
    }

    // Services:
    protected IDeviceService deviceService => scope.ServiceProvider.GetRequiredService<IDeviceService>();
    protected IKeyService keyService => scope.ServiceProvider.GetRequiredService<IKeyService>();
    protected IOwnerService ownerService => scope.ServiceProvider.GetRequiredService<IOwnerService>();
    protected IRoleService roleService => scope.ServiceProvider.GetRequiredService<IRoleService>();
    protected IDeviceRoleService deviceRoleService => scope.ServiceProvider.GetRequiredService<IDeviceRoleService>();
}
