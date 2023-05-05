using Infrastructure.Database;
using Logging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using LoggerConfigurationExtensions = Logging.LoggerConfigurationExtensions;

namespace AccessControl.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            const string appName = "Access Control Web Server";

            try
            {
                // Console logging used for troubleshooting before getting all required info for logger
                LoggerConfigurationExtensions.SetupLoggerConfiguration();
                Log.Information("Starting web host {AppName}", appName);

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((_, _, loggerConfiguration) =>
                {
                    loggerConfiguration.ConfigureBaseLogging();
                    // here add cloud logging (AppInsights for Azure, AWS something....)
                });
                
                builder.Services.RegisterApplicationServices();
                
                Log.Debug("IsDevelopment: {IsDevelopment}", builder.Environment.IsDevelopment());
                
                builder.DbConnect();

                // BUILD
                var app = builder.Build();

                if (app.Environment.IsDevelopment())
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
                
                Log.Debug("Setting Middlewares");
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

                Log.Information("Starting the Application");
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
