using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Logging;

public static class LoggerConfigurationExtensions
{
    public static void SetupConsoleLoggerConfiguration()
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureLocalLogging()
            .CreateLogger();
    }


    public static LoggerConfiguration ConfigureLocalLogging(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Async(a => a.Console(theme: AnsiConsoleTheme.Code,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message:lj}{NewLine}{Exception:j}"))
            .Enrich.FromLogContext();

        return loggerConfiguration;
    }
    public static void ConfigureProductionLogging(this LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration
            .WriteTo.Async(a => a.Seq(serverUrl: "https://access-control-seq-app.azurewebsites.net"))
            .Enrich.FromLogContext();
    }
}
