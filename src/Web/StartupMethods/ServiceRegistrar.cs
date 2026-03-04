using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.DeviceProvisioning;
using Application.IRepositories.JoinRepositories;
using Application.IRepositories.Keys;
using Application.IRepositories.Ota;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.IUnitOfWork;
using Application.Services.AccessList;
using Application.Services.Accounts;
using Application.Services.Admin;
using Application.Services.Certificate;
using Application.Services.DeviceRole;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.Services.Messaging;
using Application.Services.Ota;
using Application.Services.Owners;
using Application.Services.Provisioning;
using Application.Services.Roles;
using Application.Services.Time;
using Infrastructure.Messaging;
using Infrastructure.Repositories.Accounts;
using Infrastructure.Repositories.Devices;
using Infrastructure.Repositories.DeviceProvisioning;
using Infrastructure.Repositories.JoinRepositories;
using Infrastructure.Repositories.Keys;
using Infrastructure.Repositories.Ota;
using Infrastructure.Repositories.Owners;
using Infrastructure.Repositories.Roles;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

namespace AccessControl.Web.StartupMethods;

public static class ServiceRegistrar
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        RegisterBlazorServices(services);
        RegisterMudBlazorServices(services);
        RegisterServices(services);
    }
    private static void RegisterBlazorServices(IServiceCollection services)
    {
        // Blazor services
        Log.Debug("Adding Controllers with views");
        services.AddControllersWithViews();

        Log.Debug("Adding RazorPages");
        services.AddRazorPages();

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

        Log.Debug("Adding DeviceRole Repository");
        services.AddScoped<IDeviceRoleRepository, DeviceRoleRepository>();

        Log.Debug("Adding RoleKey Repository");
        services.AddScoped<IRoleKeyRepository, RoleKeyRepository>();

        Log.Debug("Adding DeviceRepository");
        services.AddScoped<IDeviceRepository, DeviceRepository>();

        Log.Debug("Adding IoTHubDeviceRepository");
        services.AddScoped<IIoTHubDeviceRepository, IoTHubDeviceRepository>();

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
        services.AddScoped<IAccountService, AccountService>(); ;

        Log.Debug("Adding OwnerRepository");
        services.AddScoped<IOwnerRepository, OwnerRepository>();

        Log.Debug("Adding OwnerService");
        services.AddScoped<IOwnerService, OwnerService>();

        Log.Debug("Adding IoTHubDeviceService");
        services.AddScoped<IIoTHubDeviceService, IoTHubDeviceService>();

        Log.Debug("Adding UnitOfWork");
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        Log.Debug("Adding DeviceRoleService");
        services.AddScoped<IDeviceRoleService, DeviceRoleService>();

        Log.Debug("Adding ServiceBusPublisher");
        services.AddScoped<IServiceBusPublisher, ServiceBusPublisher>();

        // IoT Device Services
        Log.Debug("Adding ProvisionedDeviceRepository");
        services.AddScoped<IProvisionedDeviceRepository, ProvisionedDeviceRepository>();

        Log.Debug("Adding OwnerFirmwareRepository");
        services.AddScoped<IOwnerFirmwareRepository, OwnerFirmwareRepository>();

        Log.Debug("Adding CertificateService");
        services.AddScoped<ICertificateService, CertificateService>();

        Log.Debug("Adding ProvisioningSecretProvider");
        services.AddScoped<IProvisioningSecretProvider, ProvisioningSecretProvider>();

        Log.Debug("Adding ProvisioningService");
        services.AddScoped<IProvisioningService, ProvisioningService>();

        Log.Debug("Adding OtaService");
        services.AddScoped<IOtaService, OtaService>();

        Log.Debug("Adding AccessListService");
        services.AddScoped<IAccessListService, AccessListService>();

        Log.Debug("Adding AdminSettingsService");
        services.AddScoped<IAdminSettingsService, AdminSettingsService>();
    }

    /// <summary>
    /// Registers IoT device configuration options.
    /// </summary>
    public static void RegisterIoTConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Debug("Configuring Certificate options");
        services.Configure<CertificateConfiguration>(
            configuration.GetSection(CertificateConfiguration.SectionName));

        Log.Debug("Configuring OTA options");
        services.Configure<OtaConfiguration>(
            configuration.GetSection(OtaConfiguration.SectionName));
    }
}