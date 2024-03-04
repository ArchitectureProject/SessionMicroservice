namespace SessionMicroservice.Models.DataobjectModels;

public record OrderRequest(
    CustomerInfosRequest CustomerInfos,
    List<ProductCommand> ProductCommands);
    
public record OrderApiRequest(
    CustomerInfosRequest CustomerInfos,
    string AlleyQrCode,
    List<ProductCommand> ProductCommands
);

public record OrderResponse(
    string Id,
    long PlacedAt,
    CustomerInfosResponse CustomerInfos,
    OrderLocalisation Localisation,
    List<ProductCommand> ProductCommands, 
    float TotalPrice,
    bool PaidByCustomer
);

public record CustomerInfosResponse(string UserId, string Email, string Name, string PhoneNumber);
public record CustomerInfosRequest(string Email, string Name, string PhoneNumber);
public record ProductCommand(string ProductId, int Quantity);