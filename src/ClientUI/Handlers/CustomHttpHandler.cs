using ApiShared.Authorization;
using Blazored.LocalStorage;

namespace DemoRegistrationEncryptionUsingRandomNumberAndSalt.Handlers
{
    public class CustomHttpHandler : DelegatingHandler
    {
        private readonly ILocalStorageService localStorageService;

        public CustomHttpHandler(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri!.AbsolutePath.ToLower().EndsWith("login") || request.RequestUri!.AbsolutePath.ToLower().EndsWith("signin"))
                return await base.SendAsync(request, cancellationToken);

            var token = await localStorageService.GetItemAsync<string>(AuthSharedConfiguration.AccessTokenKey, cancellationToken = default);
            if (token is not null)
                request.Headers.Add("Authorization", $"{token}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
