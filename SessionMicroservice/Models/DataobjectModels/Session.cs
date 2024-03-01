namespace SessionMicroservice.Models.DataobjectModels;

public record SessionResponse(
    string Id,
    long StartedAt,
    long? ClosedAt,
    LocalisationResponse Localisation,
    List<string> Orders,
    float TotalPrice,
    float CurrentlyPaid,
    bool ActuallyProcessingPayment
    );