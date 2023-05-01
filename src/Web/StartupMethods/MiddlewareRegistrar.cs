using Logging.Middlewares;
using Serilog;

namespace AccessControl.Web;

public static class MiddlewareRegistrar
{
    public static void RegisterAllEnvMiddlewares(this WebApplication app)
    {
        Log.Debug("Setting Exception handling middleware");
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}