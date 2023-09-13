using Newtonsoft.Json;

namespace UI;

public class HttpHelper
{
    public static async Task<ApiResponse<TResponse>> CallEndpoint<TBody, TResponse>(HttpClient httpClient, Uri url, HttpMethod method, TBody payload = null)
        where TBody : class
    {
        string jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

        var resultTask = method switch
        {
            HttpMethod.Get => httpClient.GetAsync(url),
            HttpMethod.Put => httpClient.PutAsync(url, content),
            HttpMethod.Delete => httpClient.DeleteAsync(url),
            HttpMethod.Post => httpClient.PostAsync(url, content),
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

    public class ApiResponse<TResponse>
    {
        public TResponse DeserializedResponse { get; set; }
        public HttpResponseMessage HttpResponse { get; set; }
    }

    public enum HttpMethod
    {
        Get,
        Put,
        Delete,
        Post
    }
}
