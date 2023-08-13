using AccessControl.Web.Middlewares;
using Serilog;

namespace AccessControl.Web;

public static class MiddlewareRegistrar
{
    public static void RegisterAllEnvMiddlewares(this WebApplication app)
    {
        RegisterProductionMiddlewares(app);
    }
    
    public static void RegisterProductionMiddlewares(this WebApplication app)
    {
        Log.Debug("Setting Exception handling middleware");
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}