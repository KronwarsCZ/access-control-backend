using Newtonsoft.Json;

namespace AccessControl.ClientUI;

public static class HttpHelpers
{
    public static async Task<ApiResponse<TResponse>> CallEndpoint<TBody, TResponse>(HttpClient httpClient, Uri url, HttpMethod method, TBody? payload = null)
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
        var responseBody = await result.Content.ReadAsStringAsync();

        if (!result.IsSuccessStatusCode)
        {
            var errorMessage = "Request failed";
            try
            {
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
                if (errorObj != null && errorObj.TryGetValue("Error message", out var msg))
                    errorMessage = msg;
            }
            catch { }
            throw new HttpRequestException(errorMessage, null, result.StatusCode);
        }

        var deserializedData = JsonConvert.DeserializeObject<TResponse>(responseBody);

        return new ApiResponse<TResponse>
        {
            DeserializedResponse = deserializedData!,
            HttpResponse = result
        };
    }

    public class ApiResponse<TResponse>
    {
        public TResponse DeserializedResponse { get; set; } = default!;
        public HttpResponseMessage HttpResponse { get; set; } = null!;
    }

    public enum HttpMethod
    {
        Get,
        Put,
        Delete,
        Post
    }
}