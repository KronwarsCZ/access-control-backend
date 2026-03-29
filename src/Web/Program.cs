using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AccessControl.Web.Api.V1.Auth;
using AccessControl.Web.StartupMethods;
using Application.Services.Accounts;
using Core.Entities.Accounts;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;

namespace AccessControl.Web;

public partial class Program
{
    public static int Main(string[] args)
    {
        const string appName = "Access Control Web Server";

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            if (builder.Environment.IsProduction())
            {
                Log.Logger = CreateProductionSerilogLogger();
            }
            else
            {
                Log.Logger = CreateLocalSerilogLogger();
            }
                
            builder.ConfigureKestrelTls();
            
            builder.Services.RegisterApplicationServices();
            builder.Services.RegisterIoTConfiguration(builder.Configuration);
                
            Log.Debug("IsDevelopment: {IsDevelopment}", builder.Environment.IsDevelopment());

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Test")
            {
                builder.DbConnect();
            }
            else
            {
                builder.Services.AddDbContext<AccessControlDbContext>(o =>
                    o.UseInMemoryDatabase("local_test_db_Constant")
                );
            }
            
            
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = AuthParameters.TokenValidationParameters;
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var email = context.Principal?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                                    ?? context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                        if (email is null)
                        {
                            context.Fail("Invalid token: missing email claim");
                            return;
                        }

                        var tokenVersion = int.Parse(context.Principal?.FindFirst("TokenVersion")?.Value ?? "-1");

                        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                        var cacheKey = $"account:{email}";

                        if (cache.TryGetValue<Account>(cacheKey, out var cachedAccount) && cachedAccount.TokenVersion == tokenVersion)
                        {
                            context.HttpContext.Items["AuthenticatedAccount"] = cachedAccount;
                            return;
                        }

                        try
                        {
                            var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();
                            var account = await accountService.GetAccountByEmailAsync(email, CancellationToken.None);
                            if (account is null || account.TokenVersion != tokenVersion)
                            {
                                context.Fail("Token has been revoked");
                                return;
                            }

                            cache.Set(cacheKey, account, TimeSpan.FromSeconds(30));
                            context.HttpContext.Items["AuthenticatedAccount"] = account;
                        }
                        catch
                        {
                            context.Fail("Token validation failed");
                        }
                    }
                };
            });
            builder.Services.AddAuthorization(options => options.AddPolicy(IdentityData.AdminUserPolicyName, p => p.RequireClaim(IdentityData.AdminUserClaimName, "true")));
            
            
            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "AccessControl API", 
                    Version = "v1" 
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    In = ParameterLocation.Header, 
                    Description = "Please insert your JWT you obtained after logging in",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey 
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    { 
                        new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference 
                            { 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" 
                            } 
                        },
                        new string[] { } 
                    } 
                });
            });

            // BUILD
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseBlazorFrameworkFiles();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapRazorPages();
            
            
            
            
            if (app.Environment.IsDevelopment())
            {
                // Initialize and migrate the database
                using (var scope = app.Services.CreateScope())
                using (var context = scope.ServiceProvider.GetRequiredService<AccessControlDbContext>())
                {
                    context.Database.Migrate();
                }
                    
                Log.Debug("Using UseDeveloperExceptionPage");
                app.UseDeveloperExceptionPage();
            }

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
            if (allowedOrigins.Length > 0)
            {
                Log.Debug("Setting CORS with allowed origins: {Origins}", string.Join(", ", allowedOrigins));
                app.UseCors(corsPolicyBuilder =>
                {
                    corsPolicyBuilder.WithOrigins(allowedOrigins);
                    corsPolicyBuilder.AllowAnyHeader();
                    corsPolicyBuilder.AllowAnyMethod();
                    corsPolicyBuilder.AllowCredentials();
                });
            }
            else
            {
                Log.Warning("No CORS origins configured — CORS requests will be rejected");
                app.UseCors();
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
        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        var telemetryConfig = TelemetryConfiguration.CreateDefault();
        telemetryConfig.ConnectionString = connectionString;

        var configuration = new Serilog.LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces);

        return configuration.CreateLogger();
    } 
    private static Serilog.ILogger CreateLocalSerilogLogger()
    {
        var configuration = new Serilog.LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .WriteTo.Console();

        return configuration.CreateLogger();
    }
}

// Needed for integration tests to pick up the correct Program class and work properly
public partial class Program { }