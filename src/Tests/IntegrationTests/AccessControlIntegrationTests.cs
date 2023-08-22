using AccessControl.Web;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.UnitOfWorkStuff_to_sort;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class AccessControlIntegrationTests
{
    private WebApplicationFactory<Program> webApplicationFactory;
    protected IServiceScope scope;
    protected DataGenerator data;
    protected IUnitOfWork unitOfWork;
    protected HttpClient _httpClient;

    [OneTimeSetUp]
    public void Setup()
    {
        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        webApplicationFactory = new WebApplicationFactory<Program>();
        _httpClient = webApplicationFactory.CreateDefaultClient();
        scope = webApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();


        data = GetDataGeneratorService();
    }

    private DataGenerator GetDataGeneratorService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<DataGenerator>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider.GetRequiredService<DataGenerator>();        
    }
    
    // Services:
    protected IDeviceService deviceService => scope.ServiceProvider.GetRequiredService<IDeviceService>();
    protected IKeyService keyService => scope.ServiceProvider.GetRequiredService<IKeyService>();

}
