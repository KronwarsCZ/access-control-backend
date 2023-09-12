using ApiShared;
using ApiShared.Services;
using BlazingChat.Shared.Services;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


if(builder.HostEnvironment.IsDevelopment()){
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7036") });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://accesscontrolapp.azurewebsites.net/") });
}

builder.Services.AddScoped<ILoginViewModel, LoginViewModel>();



// blazored services
builder.Services.AddBlazoredLocalStorage();

// authetication & authorization
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAccessTokenService, WebAppAccessTokenService>();






builder.Services.AddMudServices();

await builder.Build().RunAsync();


