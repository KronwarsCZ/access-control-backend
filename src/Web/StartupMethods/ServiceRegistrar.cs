using Application.IRepositories;
using Application.Services.Devices;
using Application.Services.Time;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace AccessControl.Web;

public static class ServiceRegistrar
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        // Blazor services
        Log.Debug("Adding RazorPages");
        services.AddRazorPages();
        
        Log.Debug("Adding serverside Blazor");
        services.AddServerSideBlazor();
        
        Log.Debug("Setting up blazor root directory");
        services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/BlazorUI/Pages");

        // Services
        Log.Debug("ConfigureServices => Setting AddControllers");
        services.AddControllers();

        Log.Debug("ConfigureServices => Setting AddHealthChecks");
        services.AddHealthChecks();

        Log.Debug("ConfigureServices => Setting AddSwaggerGen");
        services.AddSwaggerGen();
        
        Log.Debug("Adding DeviceRepository");
        services.AddScoped<IDeviceRepository, DeviceRepository>();

        Log.Debug("Adding DeviceService");
        services.AddScoped<IDeviceService, DeviceService>();

        Log.Debug("Adding CurrentUtc");
        services.AddSingleton<CurrentUtc>();

        Log.Debug("Adding TimeFreezeService");
        services.AddScoped<ITimeService>(provider => new TimeFreezeService(provider.GetRequiredService<CurrentUtc>()));
    }
}