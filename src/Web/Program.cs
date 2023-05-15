using Serilog;

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

        private static Serilog.ILogger CreateProductionSerilogLogger()
        {
            var configuration = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Async(a => a.Seq(serverUrl: "https://access-control-seq-data-app.azurewebsites.net/"));

            return configuration.CreateLogger();
        } 
        private static Serilog.ILogger CreateLocalSerilogLogger()
        {
            var configuration = new Serilog.LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            return configuration.CreateLogger();
        }
    }
}
