using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SessionMicroservice.Helpers;
using SignalRSwaggerGen.Attributes;

namespace SessionMicroservice.Controllers.Hubs;

[SignalRHub("/hubs/notifications")]
[Authorize]
public class NotificationHub : Hub
{
    public async Task SubToSession(string qrCode)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, qrCode);
        }
        catch (Exception ex)
        {
            throw new AppException("Error while connecting to the notification hub : " + ex.Message, 500);
        }
    }
}