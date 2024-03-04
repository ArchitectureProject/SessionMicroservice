using SessionMicroservice.Extensions;
using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;

namespace SessionMicroservice.Services;

public interface IOrderApiService
{
    Task<OrderResponse> SendOrderAsync(OrderApiRequest model);
    Task<List<OrderResponse>> GetUnpaidOrdersByUserAsync(string userId);
    Task MarkOrdersAsPaidAsync(List<string> orderIds);
}
public class OrderApiService : IOrderApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() =>
        _httpClientFactory.CreateClient("OrderApi");

    public async Task<OrderResponse> SendOrderAsync(OrderApiRequest model) 
        => await CreateClient().PostToApiAsync<OrderApiRequest,OrderResponse>("/order", model);

    public async Task<List<OrderResponse>> GetUnpaidOrdersByUserAsync(string userId) 
        => await CreateClient().GetFromApiAsync<List<OrderResponse>>($"/order/unpaid/{userId}");

    public async Task MarkOrdersAsPaidAsync(List<string> orderIds)
        => await CreateClient().PutToApiAsync<List<string>, ClientExtensions.NothingInReturn>("/order/payed", orderIds);
}