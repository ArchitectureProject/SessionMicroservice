using Microsoft.AspNetCore.SignalR;
using SessionMicroservice.Controllers.Hubs;

namespace SessionMicroservice.Services;

public interface INotificationService
{
    Task SendNotificationToSession(string qrCode, string message);
}
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendNotificationToSession(string qrCode, string message)
    {
        await _hubContext.Clients.Group(qrCode).SendAsync("Notification", message);
    }
}