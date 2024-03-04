using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;
using SessionMicroservice.Services;

namespace SessionMicroservice.Controllers;

[ApiController]
[Route("[controller]"), Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    
    [HttpGet("{qrCode}"), Authorize(Roles = "CUSTOMER,AGENT")]
    public async Task<SessionResponse> GetSession(string qrCode)
        => await _sessionService.GetOrCreateSession(qrCode);
    
    [HttpPost("actualisePayment/{sessionId}"), Authorize(Roles = "AGENT")]
    public PaymentActualisationResponse ActualisePayment(string sessionId, PaymentActualisationRequest model)
        => _sessionService.ActualisePayment(sessionId, model);
    
    [HttpPost("attemptPayment/{qrCode}"), Authorize(Roles = "CUSTOMER")]
    public async Task<PaymentResponse> AttemptPayment(string qrCode, PaymentRequest model) 
        => await _sessionService.AttemptPayment(qrCode, model, GetUserId());

    [HttpPost("SetAmountManually/{qrCode}"), Authorize(Roles = "AGENT")]
    public async Task<PaymentResponse> SetAmountManually(string qrCode, PaymentRequest model) 
        => await _sessionService.AttemptPayment(qrCode, model, GetUserId());
    
    [HttpPost("closeSession/{qrCode}"), Authorize(Roles = "AGENT")]
    public async Task<SessionResponse> TryCloseSession(string qrCode)
        => await _sessionService.TryCloseSession(qrCode);
    
    [HttpPost("order/{qrCode}"), Authorize(Roles = "CUSTOMER")]
    public async Task<OrderResponse> Order(string qrCode, OrderRequest model) 
        => await _sessionService.Order(qrCode, model);
    
    private string GetUserId() =>
        User.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value ??
        throw new AppException("Error while getting userId from token", 500);
}