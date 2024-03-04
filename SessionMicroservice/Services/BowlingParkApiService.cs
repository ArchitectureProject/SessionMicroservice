using SessionMicroservice.Extensions;
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
        => await CreateClient().GetFromApiAsync<BowlingParkDataFromQrCode>($"/BowlingPark/fromQrCode/{qrCode}"); 
    
    private HttpClient CreateClient() =>
        _httpClientFactory.CreateClient("BowlingParkApi");
}