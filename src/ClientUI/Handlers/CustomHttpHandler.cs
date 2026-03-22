using System.Net;
using ApiShared.Authorization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace DemoRegistrationEncryptionUsingRandomNumberAndSalt.Handlers
{
    public class CustomHttpHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;

        public CustomHttpHandler(ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath.ToLower();
            if (path.EndsWith("login") || path.EndsWith("signin"))
                return await base.SendAsync(request, cancellationToken);

            var token = await _localStorageService.GetItemAsync<string>(AuthSharedConfiguration.AccessTokenKey, cancellationToken = default);
            if (token is not null)
                request.Headers.Add("Authorization", $"{token}");

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorageService.RemoveItemAsync(AuthSharedConfiguration.AccessTokenKey);
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }
    }
}
