using SessionMicroservice.Models.DataobjectModels;
using SessionMicroservice.Models.Entities;

namespace SessionMicroservice.Extensions;

public static class SessionExtensions
{
    public static SessionResponse ToSessionResponse(this Session session, BowlingParkDataFromQrCode data)
        => new (
            Id: session.Id,
            StartedAt: session.StartedAt,
            ClosedAt: session.ClosedAt,
            Localisation: new LocalisationResponse(
                BowlingParkId: data.BowlingParkId,
                AlleyNumber: data.AlleyNumber,
                QrCode: session.LocationQrCode
            ),
            Orders: session.Orders,
            TotalPrice: session.TotalPrice,
            CurrentlyPaid: session.CurrentlyPaid,
            ActuallyProcessingPayment: session.ActuallyProcessingPayment
        );
}