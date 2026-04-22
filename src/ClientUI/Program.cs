using System.Globalization;
using Blazored.LocalStorage;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt.Handlers;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt.Provider;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7068/") });

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddMudServices();
builder.Services.AddHttpClient("client", options => 
{
    options.BaseAddress = baseAddress;
}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Admin", p => p.RequireClaim("admin", "true"));
});
builder.Services.AddScoped<CustomHttpHandler>();

var host = builder.Build();

try
{
    var js = (IJSInProcessRuntime)host.Services.GetRequiredService<IJSRuntime>();
    var browserLang = js.Invoke<string>("eval", "navigator.language || navigator.userLanguage || 'en-US'");
    var culture = new CultureInfo(browserLang);
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}
catch
{
    // fall back to invariant if browser didn't surface a usable culture
}

await host.RunAsync();
