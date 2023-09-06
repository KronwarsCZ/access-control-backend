using AccessControl.Web.Authentication;
using Application.Services.Accounts;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.Services.Roles;
using Application.Services.Time;
using Application.UnitOfWorkStuff_to_sort;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

namespace AccessControl.Web.StartupMethods;

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
        // Services
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

        Log.Debug("Adding DeviceService");
        services.AddScoped<IDeviceService, DeviceService>();

        Log.Debug("Adding KeyService");
        services.AddScoped<IKeyService, KeyService>();

        Log.Debug("Adding RoleService");
        services.AddScoped<IRoleService, RoleService>();

        Log.Debug("Adding AccountService");
        services.AddScoped<IAccountService, AccountService>(); ;

        Log.Debug("Adding OwnerService");
        services.AddScoped<IOwnerService, OwnerService>();

        Log.Debug("Adding UnitOfWork");
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}