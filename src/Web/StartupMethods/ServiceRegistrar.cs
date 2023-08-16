using AccessControl.Web.Authentication;
using ApiIntegrationTests;
using Application.IRepositories;
using Application.Services;
using Application.Services.DevicesRoles;
using Application.Services.Owner;
using Application.Services.RolesKeys;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

namespace AccessControl.Web;

public static class ServiceRegistrar
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        RegisterAuthenticationServices(services);
        RegisterBlazorServices(services);
        RegisterMudBlazorServices(services);
        RegisterServices(services);
    }

    private static void RegisterAuthenticationServices(IServiceCollection services)
    {
        // Authentication
        Log.Debug("Adding Authentication Core");
        services.AddAuthenticationCore();

        Log.Debug("Adding ProtectedSessionStorage");
        services.AddScoped<ProtectedSessionStorage>();

        Log.Debug("Adding CustomAuthenticationStateProvider as a AuthenticationStateProvider");
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
    }

    private static void RegisterBlazorServices(IServiceCollection services)
    {
        // Blazor services
        Log.Debug("Adding RazorPages");
        services.AddRazorPages();

        Log.Debug("Adding serverside Blazor");
        services.AddServerSideBlazor();

        Log.Debug("Setting up blazor root directory");
        services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/BlazorUI/Pages");

        Log.Debug("Adding MudBlazor Services");
        services.AddMudServices();
    }

    private static void RegisterMudBlazorServices(IServiceCollection services)
    {
        // Blazor services
        Log.Debug("Configuring MudBlazor Services");
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopEnd;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Services and repositories
        
        Log.Debug("Adding DBSaveContextRepository");
        services.AddScoped<IDBContextSaveRepository, DBContextSaveRepository>();

        Log.Debug("Adding CurrentUtc");
        services.AddSingleton<CurrentUtc>();

        Log.Debug("Adding TimeFreezeService");
        services.AddScoped(provider => new TimeFreezeService(provider.GetRequiredService<CurrentUtc>()));
        
        Log.Debug("ConfigureServices => Setting AddControllers");
        services.AddControllers();

        Log.Debug("ConfigureServices => Setting AddHealthChecks");
        services.AddHealthChecks();

        Log.Debug("ConfigureServices => Setting AddSwaggerGen");
        services.AddSwaggerGen();

        Log.Debug("Adding OwnerRepository");
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        
        Log.Debug("Adding DeviceRepository");
        services.AddScoped<IDeviceRepository, DeviceRepository>();

        Log.Debug("Adding DeviceService");
        services.AddScoped<IDeviceService, DeviceService>();
        
        Log.Debug("Adding KeyRepository");
        services.AddScoped<IKeyRepository, KeyRepository>();
        
        Log.Debug("Adding KeyService");
        services.AddScoped<IKeyService, KeyService>();
        
        Log.Debug("Adding RoleRepository");
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        Log.Debug("Adding RoleService");
        services.AddScoped<IRoleService, RoleService>();

        Log.Debug("Adding AccountRepository");
        services.AddScoped<IAccountRepository, AccountRepository>();

        Log.Debug("Adding AccountService");
        services.AddScoped<IAccountService, AccountService>();
        
        Log.Debug("Adding RolesKeysRepository");
        services.AddScoped<IRolesKeysRepository, RolesKeysRepository>();

        Log.Debug("Adding RolesKeysService");
        services.AddScoped<IRolesKeysService, RolesKeysService>();
        
        Log.Debug("Adding DevicesRolesService");
        services.AddScoped<IDevicesRolesService, DevicesRolesService>();

        Log.Debug("Adding DevicesRolesRepository");
        services.AddScoped<IDevicesRolesRepository, DevicesRolesRepository>();
        
        Log.Debug("Adding OwnerRepository");
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        
        Log.Debug("Adding OwnerService");
        services.AddScoped<IOwnerService, OwnerService>();        
        
        Log.Debug("Adding SampleDataService");
        services.AddScoped<SampleDataService>();
    }
}
