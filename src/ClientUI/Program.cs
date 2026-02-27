using Blazored.LocalStorage;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt.Handlers;
using DemoRegistrationEncryptionUsingRandomNumberAndSalt.Provider;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomHttpHandler>();
await builder.Build().RunAsync();
