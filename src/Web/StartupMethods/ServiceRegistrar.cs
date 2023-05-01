using Application.IRepositories;
using Application.Services;
using Infrastructure.Repositories;
using Serilog;

namespace AccessControl.Web;

public static class ServiceRegistrar
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        Log.Debug("ConfigureServices => Setting AddControllers");
        services.AddControllers();

        Log.Debug("ConfigureServices => Setting AddHealthChecks");
        services.AddHealthChecks();

        Log.Debug("ConfigureServices => Setting AddSwaggerGen");
        services.AddSwaggerGen();
        
        Log.Debug("Adding INecoRepository");
        services.AddScoped<INecoRepository, NecoRepository>();

        Log.Debug("Adding NecoService");
        services.AddScoped<INecoService, NecoService>();
    }
}