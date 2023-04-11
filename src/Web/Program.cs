using Application.IRepositories;
using Application.Services;
using Infrastructure.Repositories;
using Logging;
using Logging.Middlewares;
using Serilog;
using LoggerConfigurationExtensions = Logging.LoggerConfigurationExtensions;


const string appName = "Access Control Web Server";

LoggerConfigurationExtensions.SetupLoggerConfiguration();
Log.Information("Starting web host {AppName}", appName);

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

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

var app = builder.Build();
var env = app.Environment;

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
        
Log.Debug("Setting Exception handling middleware");
app.UseMiddleware<ExceptionHandlingMiddleware>();
/*
app.UseAuthentication(x => {x.}); // kdo jsi
app.UseAuthorization(); // co muzes delat
*/
Log.Debug("Setting UseSwagger");
app.UseSwagger();

Log.Debug("Setting UseSwaggerUI");
app.UseSwaggerUI();

Log.Debug("Setting UseHttpsRedirection");
app.UseHttpsRedirection();

Log.Debug("Setting UseRouting");
app.UseRouting();

Log.Debug("Setting UseEndpoints");
app.UseEndpoints(endpoints =>
{
    Log.Debug("Setting endpoints => MapControllers");
    endpoints.MapControllers();

    Log.Debug("Setting endpoints => add health check");
    endpoints.MapHealthChecks("/health");
});


app.Run();
/*

        const string appName = "Access Control Web Server";

        try
        {
            // Console logging for used for troubleshooting before getting all required info for logger
            LoggerConfigurationExtensions.SetupLoggerConfiguration();

            Log.Information("Starting web host {AppName}", appName);

            Host.CreateDefaultBuilder()
                .UseSerilog((_, _, loggerConfiguration) =>
                {
                    loggerConfiguration.ConfigureBaseLogging();
                    // here add cloud logging (AppInsights for Azure, AWS something....)
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();

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



 */
