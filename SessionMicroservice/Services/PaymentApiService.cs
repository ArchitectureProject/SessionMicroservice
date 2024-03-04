using SessionMicroservice.Extensions;
using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;

namespace SessionMicroservice.Services;

public interface IPaymentApiService
{
    Task<PaymentApiResponse> SendPaymentAttemptAsync(PaymentApiRequest model);
}
public class PaymentApiService : IPaymentApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentApiService(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public async Task<PaymentApiResponse> SendPaymentAttemptAsync(PaymentApiRequest model)
        => await CreateClient().PostToApiAsync<PaymentApiRequest, PaymentApiResponse>("/payment-attempt", model);
    
    private HttpClient CreateClient() =>
        _httpClientFactory.CreateClient("PaymentApi");
}