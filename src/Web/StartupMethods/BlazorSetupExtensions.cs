using Serilog;

namespace AccessControl.Web.StartupMethods;

public static class BlazorSetupExtensions
{
    public static void BlazorSetup(this WebApplication app)
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        Log.Debug("Use HSTS");
        app.UseHsts();
                
        Log.Debug("Use static files");
        app.UseStaticFiles();

        Log.Debug("Map fallback to page");
        app.MapFallbackToFile("index.html");
    }
}