namespace SessionMicroservice.Models.DataobjectModels;

public record BowlingParkErrorResponse(string Error);
public record BowlingParkDataFromQrCode(string BowlingParkId, int AlleyNumber);