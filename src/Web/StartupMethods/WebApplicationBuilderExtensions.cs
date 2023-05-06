using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AccessControl.Web;

public static class WebApplicationBuilderExtensions
{
    public static void DbConnect(this WebApplicationBuilder builder)
    {
        var connectionString = String.Empty;
        
        if (builder.Environment.IsDevelopment())
        {
            Log.Debug("Connecting to local database");
            builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
            connectionString = builder.Configuration.GetConnectionString("LocalConnectionString");
        }
        else
        {
            Log.Debug("Connecting to production Azure database");
            connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_azor");
        }
        Log.Debug("Registering AccessControlDbContext");
        builder.Services.AddDbContext<AccessControlDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                optionsAction => optionsAction.EnableRetryOnFailure().MigrationsAssembly("Web")
            )
        );
    }
}