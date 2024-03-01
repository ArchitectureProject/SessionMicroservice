using Microsoft.AspNetCore.Mvc;
using SessionMicroservice.Models.DataobjectModels;
using SessionMicroservice.Services;

namespace SessionMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    [HttpGet("{qrCode}")]
    public async Task<SessionResponse> GetSession(string qrCode)
        => await _sessionService.GetOrCreateSession(qrCode);
}