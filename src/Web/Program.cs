using Infrastructure.Database;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using LoggerConfigurationExtensions = Logging.LoggerConfigurationExtensions;

namespace AccessControl.Web
{
    class Program
    {
        static int Main(string[] args)
        {
            const string appName = "Access Control Web Server";

            try
            {
                // Console logging for used for troubleshooting before getting all required info for logger

                LoggerConfigurationExtensions.SetupLoggerConfiguration();
                Log.Information("Starting web host {AppName}", appName);

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((_, _, loggerConfiguration) =>
                {
                    loggerConfiguration.ConfigureBaseLogging();
                    // here add cloud logging (AppInsights for Azure, AWS something....)
                });
                builder.Services.RegisterApplicationServices();

                
                var connection = String.Empty;
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
                    connection = builder.Configuration.GetConnectionString("LocalConnectionString");
                }
                else
                {
                    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
                }
                Log.Debug("Registering PersonDbContext");
                builder.Services.AddDbContext<PersonDbContext>(options => options.UseSqlServer(connection));

                var app = builder.Build();
                var env = app.Environment;
                
                

                // Migrate latest database changes during startup
                Log.Debug("Creating Serivce Scope");
                using (var scope = app.Services.CreateScope())
                {
                    Log.Debug("GetDbContext");
                    var dbContext = scope.ServiceProvider.GetRequiredService<PersonDbContext>();
                    
                    
                    RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
                    databaseCreator.CreateTables();
                    
                }

                
                /*


                */
                
                /*

                
                                using(var scope = app.Services.CreateScope())
                {
                    var context = app.Services.GetRequiredService<PersonDbContext>();
                    context.Database.EnsureCreated();
                }
                */
                
                
                
                
                
                Log.Debug("IsDevelopment: {IsDevelopment}", env.IsDevelopment());
                if (env.IsDevelopment())
                {
                    Log.Debug("Using UseDeveloperExceptionPage");
                    app.UseDeveloperExceptionPage();

                    Log.Debug("Setting cors => allow *");
                    app.UseCors(builder =>
                    {
                        builder.SetIsOriginAllowed(_ => true);
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
                }

                app.RegisterAllEnvMiddlewares();
                
                
                

                
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

                Log.Debug("Starting the Application");

                app.Run();

                Log.Information("Ending web host {AppName}", appName);
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
    }
}
