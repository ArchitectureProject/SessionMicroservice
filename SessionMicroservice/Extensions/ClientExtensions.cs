using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;

namespace SessionMicroservice.Extensions;

public static class ClientExtensions
{
    public static async Task<TR> PostToApiAsync<T, TR>(this HttpClient client, string requestUri, T model)
    {
        var response = await client.PostAsJsonAsync(requestUri, model);
        
        if (!response.IsSuccessStatusCode)
            throw new AppException($"Error while sending api call POST to {requestUri} : " + response.ReasonPhrase, 
                (int)response.StatusCode);
        
        if (typeof(TR) == typeof(NothingInReturn))
            return default;
        
        return await response.Content.ReadFromJsonAsync<TR>() 
               ?? throw new AppException("Error while deserializing " + nameof(TR), 500);
    }
    
    public static async Task<TR> PutToApiAsync<T, TR>(this HttpClient client, string requestUri, T model)
    {
        var response = await client.PutAsJsonAsync(requestUri, model);
        
        if (!response.IsSuccessStatusCode)
            throw new AppException($"Error while sending api call PUT to {requestUri} : " + response.ReasonPhrase, 
                (int)response.StatusCode);
        
        if (typeof(TR) == typeof(NothingInReturn))
            return default;
        
        return await response.Content.ReadFromJsonAsync<TR>() 
               ?? throw new AppException("Error while deserializing " + nameof(TR), 500);
    }
    
    public static async Task<TR> GetFromApiAsync<TR>(this HttpClient client, string requestUri)
    {
        var response = await client.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
            throw new AppException($"Error while sending api call GET from {requestUri} : " + response.ReasonPhrase, 
                (int)response.StatusCode);
        
        if (typeof(TR) == typeof(NothingInReturn))
            return default;
        
        return await response.Content.ReadFromJsonAsync<TR>() 
               ?? throw new AppException("Error while deserializing " + nameof(TR), 500);
    }

    public record NothingInReturn;
}