using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;

namespace SessionMicroservice.Services;

public interface IBowlingParkApiService
{
    Task<BowlingParkDataFromQrCode> GetFromQrCode(string qrCode);
}

public class BowlingParkApiService : IBowlingParkApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BowlingParkApiService(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public async Task<BowlingParkDataFromQrCode> GetFromQrCode(string qrCode)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"BowlingPark/fromQrCode/{qrCode}");

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<BowlingParkDataFromQrCode>() ??
                   throw new AppException("Error while deserializing BowlingPark data from QR code", 500);
        
        var error = await response.Content.ReadFromJsonAsync<BowlingParkErrorResponse>();
        throw new AppException("Error while getting BowlingPark data from QR code : " + error?.Error ?? "Unknown error", (int)response.StatusCode);
    }
    
    private HttpClient CreateClient() =>
        _httpClientFactory.CreateClient("BowlingParkApi");
}