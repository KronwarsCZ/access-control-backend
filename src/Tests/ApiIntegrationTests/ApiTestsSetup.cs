using ApiShared.Dto.Auth;
using IntegrationTests;
using Newtonsoft.Json;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public abstract class ApiTestsSetup : AccessControlIntegrationTests
{
    private async Task AuthenticateHttpClientAsync(string email, string plaintextPassword)
    {
        var loginParameters = new LoginParameters
        {
            Email = email,
            Password = plaintextPassword
        };
        var loginResponse = await CallEndpointWithoutAuthorization<LoginParameters, LoginResult>(LoginUri, HttpMethod.Post, loginParameters);
        // pridat Authorization header
        _httpClient.DefaultRequestHeaders.Add("Authorization", loginResponse.DeserializedResponse.Token ?? "some invalid token");
    }
    
    protected async Task<ApiResponse<TResponse>> CallEndpointWithoutAuthorization<TBody, TResponse>(Uri url, HttpMethod method, TBody payload = null)
        where TBody : class
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
        
        // smazat Authorization header
        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        var resultTask = method switch
        {
            HttpMethod.Get => _httpClient.GetAsync(url),
            HttpMethod.Put => _httpClient.PutAsync(url, content),
            HttpMethod.Delete => _httpClient.DeleteAsync(url),
            HttpMethod.Post => _httpClient.PostAsync(url, content),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
        var result = await resultTask;
        
        var deserializedData = JsonConvert.DeserializeObject<TResponse>(await result.Content.ReadAsStringAsync());
        
        return new ApiResponse<TResponse>
        {
            DeserializedResponse = deserializedData,
            HttpResponse = result
        };
    }
    
    protected async Task<ApiResponse<TResponse>> CallEndpoint<TBody, TResponse>(Uri url, string email, HttpMethod method, TBody payload = null)
        where TBody : class
    {
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
        
        // smazat Authorization header
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        // login se test accountem se spravnym ownerem
        await AuthenticateHttpClientAsync(email, TestPass);

        var resultTask = method switch
        {
            HttpMethod.Get => _httpClient.GetAsync(url),
            HttpMethod.Put => _httpClient.PutAsync(url, content),
            HttpMethod.Delete => _httpClient.DeleteAsync(url),
            HttpMethod.Post => _httpClient.PostAsync(url, content),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
        var result = await resultTask;
        
        var deserializedData = JsonConvert.DeserializeObject<TResponse>(await result.Content.ReadAsStringAsync());
        
        return new ApiResponse<TResponse>
        {
            DeserializedResponse = deserializedData,
            HttpResponse = result
        };
    }

    protected class ApiResponse<TResponse>
    {
        public TResponse DeserializedResponse { get; set; }
        public HttpResponseMessage HttpResponse { get; set; }
    }
}

