using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;
using UI.Handlers;
using UI.Provider;
using UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<IUserService, UserService>();

var baseAddress = new Uri("https://localhost:7036/");
if (builder.HostEnvironment.IsProduction())
{
    baseAddress = new Uri("https://accesscontrolapp.azurewebsites.net/");
}


builder.Services.AddHttpClient("client", options =>
{
    options.BaseAddress = baseAddress;
}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddBlazoredLocalStorage(); 
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomHttpHandler>();
await builder.Build().RunAsync();
