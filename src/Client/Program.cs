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
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7068/") });

builder.Services.AddHttpClient("MyApi", options =>
{
    options.BaseAddress = new Uri("https://localhost:7068/");
}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomHttpHandler>();
await builder.Build().RunAsync();
