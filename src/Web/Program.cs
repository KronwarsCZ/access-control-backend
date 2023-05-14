using Logging;
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
                // Console logging before creation of WebApplicationBuilder
                LoggerConfigurationExtensions.SetupConsoleLoggerConfiguration();

                Log.Debug("Starting web host {AppName}", appName);

                var builder = WebApplication.CreateBuilder(args);


                if (builder.Environment.IsProduction())
                {
                    Log.Debug("Setting up production logging to seq");
                    builder.Host.UseSerilog((_, _, loggerConfiguration) =>
                    {
                        loggerConfiguration.ConfigureProductionLogging();
                    });
                }
                
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
                
#pragma warning disable ASP0014
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/health");
                });
#pragma warning restore ASP0014

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
    }
}
