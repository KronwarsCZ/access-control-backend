using AccessControl.Web;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class AccessControlIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    protected WebApplicationFactory<Program> webApplicationFactory;
    protected IServiceScope scope;
    private IUnitOfWork unitOfWork;
    protected HttpClient _httpClient;

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
        return await ownerService.GetByIdAsync(owner.Id, new CancellationToken());
    }

    // Services:
    protected IDeviceService deviceService => scope.ServiceProvider.GetRequiredService<IDeviceService>();
    protected IKeyService keyService => scope.ServiceProvider.GetRequiredService<IKeyService>();
    protected IOwnerService ownerService => scope.ServiceProvider.GetRequiredService<IOwnerService>();

}
