using IntegrationTests;
using Newtonsoft.Json;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace ApiIntegrationTests;

public abstract class ApiTestsSetup : AccessControlIntegrationTests
{
    protected async Task<ApiResponse<TResponse>> CallEndpoint<TBody, TResponse>(Uri url, HttpMethod method, TBody payload = null)
        where TBody : class
    {
        string jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

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

