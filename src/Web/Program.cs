using EFCore.AutomaticMigrations;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace AccessControl.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            const string appName = "Access Control Web Server";

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();
                
                Log.Logger = CreateProductionSerilogLogger();
                
                if (builder.Environment.IsDevelopment())
                {
                    Log.Logger = CreateLocalSerilogLogger();
                }
                
                builder.Services.RegisterApplicationServices();
                
                Log.Debug("IsDevelopment: {IsDevelopment}", builder.Environment.IsDevelopment());
                
                builder.DbConnect();

                // BUILD
                var app = builder.Build();
                
                if (app.Environment.IsDevelopment())
                {
                    // TODO: add seeding
                    using (var scope = app.Services.CreateScope())
                    using (var context = scope.ServiceProvider.GetService<AccessControlDbContext>())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        try
                        {
                            context.Database.ExecuteSqlRaw(DatabaseSeeding.sql);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    
                    Log.Debug("Using UseDeveloperExceptionPage");
                    app.UseDeveloperExceptionPage();

                    Log.Debug("Setting cors => allow *");
                    app.UseCors(corsPolicyBuilder =>
                    {
                        corsPolicyBuilder.SetIsOriginAllowed(_ => true);
                        corsPolicyBuilder.AllowAnyHeader();
                        corsPolicyBuilder.AllowAnyMethod();
                    });
                }

                /*
                // Migrate the database 
                using (var scope = app.Services.CreateScope())
                using (var context = scope.ServiceProvider.GetService<AccessControlDbContext>())
                {
                    try
                    {
                        // If table or column is unused it gets deleted is controlled by the ResetDatabaseSchema
                        MigrateDatabaseToLatestVersion.Execute(context, new DbMigrationsOptions { ResetDatabaseSchema = true });
                        Log.Logger.Information("Migrace databaze se povedla, yeeey!");
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Fatal(e, "zase se posrala migrace a chcipla");
                    }
                }
                */
                
                if (app.Environment.IsDevelopment())
                {
                    Log.Debug("Setting all environment Middlewares");
                    app.RegisterAllEnvMiddlewares();
                }
                else
                {
                    Log.Debug("Setting production Middlewares");
                    app.RegisterProductionMiddlewares();
                }
                
                Log.Debug("Setting UseSwagger");
                app.UseSwagger();

                Log.Debug("Setting UseSwaggerUI");
                app.UseSwaggerUI();

                Log.Debug("Setting UseHttpsRedirection");
                app.UseHttpsRedirection();

                Log.Debug("Setting UseRouting");
                app.UseRouting();
                
                Log.Debug("Setting UseAuthentication");
                app.UseAuthentication();
                
                Log.Debug("Setting UseAuthorization");
                app.UseAuthorization();
                
                Log.Debug("Setting UseEndpoints");
                
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/health");
                });

                app.BlazorSetup();

                Log.Debug("Starting the Application");
                app.Run();

                Log.Debug("Ending web host {AppName}", appName);
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly {AppName}", appName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static Serilog.ILogger CreateProductionSerilogLogger()
        {
            var apiKey = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_SEQ_API_KEY");
            var configuration = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .WriteTo.Async(a => a.Seq(
                    serverUrl: "https://access-control-seq-data-app.azurewebsites.net/",
                    apiKey: apiKey)
                );

            return configuration.CreateLogger();
        } 
        private static Serilog.ILogger CreateLocalSerilogLogger()
        {
            var configuration = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .WriteTo.Console();

            return configuration.CreateLogger();
        }
    }
}
