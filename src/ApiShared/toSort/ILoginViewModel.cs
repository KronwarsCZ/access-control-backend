using BlazingChat.Shared;
using BlazingChat.Shared.Models;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public Task<AuthenticationResponse> AuthenticateJWT();
        public Task<string> GetFacebookAppIDAndRedirectUriAsync();
        public Task<User> GetUserByJWTAsync(string jwtToken);
    }
}