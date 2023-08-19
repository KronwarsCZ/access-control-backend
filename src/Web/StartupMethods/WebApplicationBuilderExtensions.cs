using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AccessControl.Web.StartupMethods;

public static class WebApplicationExtensions
{
    public static void DbConnect(this WebApplicationBuilder builder, bool connectToTestingDatabase)
    {
        var connectionString = String.Empty;

        builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");

        if (connectToTestingDatabase)
        {
            Log.Debug("Connecting to testing database");
            connectionString = builder.Configuration.GetConnectionString("TestingConnectionString");
        }
        else if(builder.Environment.IsDevelopment())
        {
            Log.Debug("Connecting to local database");
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
            ),
            optionsLifetime: ServiceLifetime.Transient
        );
    }
}