namespace SessionMicroservice.Models.DataobjectModels;

public record LocalisationResponse(string BowlingParkId, int AlleyNumber, string QrCode);
public record OrderLocalisation(string BowlingParkId, int AlleyNumber);