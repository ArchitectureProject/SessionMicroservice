namespace SessionMicroservice.Models.DataobjectModels;

public record PaymentActualisationRequest(string PaymentId, float Amount);
public record PaymentActualisationResponse(string PaymentId, float CurrentlyPaid);
public record PaymentApiResponse(
    string Id, 
    string PaymentState, 
    string PaymentType, 
    long LaunchedAt, 
    string SessionId,
    string UserId,
    float Amount,
    CreditCardInfosRequest? CreditCardInfos);

public record PaymentApiRequest(
    string PaymentType, 
    string SessionId,
    string UserId,
    float Amount,
    CreditCardInfosRequest? CreditCardInfos
);
public record PaymentManualRequest(float Amount);
public record PaymentRequest(string PaymentType, string PaymentPartType, float Amount, CreditCardInfosRequest? CreditCardInfos);

public record CreditCardInfosRequest(string Pan, string ExpirationDate, string Cvv);

public record PaymentResponse(string PaymentId, string PaymentState);