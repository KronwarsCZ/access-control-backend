using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AccessControl.Web;

/// <summary>
/// Allows `dotnet ef` to create the DbContext at design time without
/// starting the full application (which requires TLS certs, etc.).
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccessControlDbContext>
{
    public AccessControlDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("LocalConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<AccessControlDbContext>();
        optionsBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly("Web"));

        return new AccessControlDbContext(optionsBuilder.Options);
    }
}
