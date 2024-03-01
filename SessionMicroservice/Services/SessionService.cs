using SessionMicroservice.Extensions;
using SessionMicroservice.Models.DataobjectModels;
using SessionMicroservice.Models.Entities;

namespace SessionMicroservice.Services;

public interface ISessionService
{
    public Task<SessionResponse> GetOrCreateSession(string qrCode);
}
public class SessionService : ISessionService
{
    private readonly IBowlingParkApiService _bowlingParkApiService;
    private readonly DataContext _context;

    public SessionService(IBowlingParkApiService bowlingParkApiService,
        DataContext context)
    {
        _bowlingParkApiService = bowlingParkApiService;
        _context = context;
    }

    public async Task<SessionResponse> GetOrCreateSession(string qrCode)
    {
        // Get data from BowlingPark
        var bowlingParkData = await _bowlingParkApiService.GetFromQrCode(qrCode);
        
        // Get or create session
        var session = _context.Sessions
            .FirstOrDefault(s => s.LocationQrCode == qrCode);

        if (session == null)
        {
            session = new Session
            {
                ActuallyProcessingPayment = false,
                CurrentlyPaid = 0,
                LocationQrCode = qrCode,
                Orders = new List<string>(),
                StartedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
                TotalPrice = 0,
                ClosedAt = null,
                Id = Guid.NewGuid().ToString()
            };
            
            _context.Sessions.Add(session);
        }
        
        await _context.SaveChangesAsync();

        return session.ToSessionResponse(bowlingParkData);
    }
}