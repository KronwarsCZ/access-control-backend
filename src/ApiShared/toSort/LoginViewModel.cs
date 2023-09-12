using BlazingChat.Shared;
using BlazingChat.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        private readonly HttpClient _httpClient;

        public LoginViewModel()
        {
        }
        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthenticationResponse> AuthenticateJWT()
        {
            //creating authentication request
            var authenticationRequest = new AuthenticationRequest
            {
                EmailAddress = EmailAddress,
                Password = Password,
            };

            //authenticating the request
            var httpMessageResponse = await _httpClient.PostAsJsonAsync("user/authenticatejwt", authenticationRequest);

            //sending the token to the client to store
            return await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

        public Task<string> GetFacebookAppIDAndRedirectUriAsync() =>
            _httpClient.GetStringAsync("user/getfacebookappidandredirecturi");

        public async Task<User> GetUserByJWTAsync(string jwtToken)
        {
            try
            {
                //preparing the http request
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "user/getuserbyjwt")
                {
                    Content = new StringContent(jwtToken)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }
                };

                //making the http request
                var response = await _httpClient.SendAsync(requestMessage);

                //returning the user if found
                var returnedUser = await response.Content.ReadFromJsonAsync<User>();
                if (returnedUser != null) return await Task.FromResult(returnedUser);
                else return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.GetType());
                return null;
            }            
        }
    }
}
