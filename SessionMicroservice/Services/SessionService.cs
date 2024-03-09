using Microsoft.EntityFrameworkCore;
using SessionMicroservice.Extensions;
using SessionMicroservice.Helpers;
using SessionMicroservice.Models.DataobjectModels;
using SessionMicroservice.Models.Entities;

namespace SessionMicroservice.Services;

public interface ISessionService
{
    public Task<SessionResponse> GetOrCreateSession(string qrCode);
    PaymentActualisationResponse ActualisePayment(string sessionId, PaymentActualisationRequest model);
    Task<PaymentResponse> AttemptPayment(string qrCode, PaymentRequest model, string UserId);
    Task<OrderResponse> Order(string qrCode, OrderRequest model);
    Task<SessionResponse> TryCloseSession(string qrCode);
    Task SetAmountManually(string qrCode, PaymentManualRequest model);
}
public class SessionService : ISessionService
{
    private readonly IBowlingParkApiService _bowlingParkApiService;
    private readonly IPaymentApiService _paymentApiService;
    private readonly IOrderApiService _orderApiService;
    private readonly DataContext _context;
    private readonly ILogger<SessionService> _logger;
    private readonly INotificationService _notificationService;

    public SessionService(IBowlingParkApiService bowlingParkApiService,
        DataContext context, ILogger<SessionService> logger, 
        IPaymentApiService paymentApiService, 
        IOrderApiService orderApiService, 
        INotificationService notificationService)
    {
        _bowlingParkApiService = bowlingParkApiService;
        _context = context;
        _logger = logger;
        _paymentApiService = paymentApiService;
        _orderApiService = orderApiService;
        _notificationService = notificationService;
    }

    public async Task<SessionResponse> GetOrCreateSession(string qrCode)
    {
        // Get data from BowlingPark
        var bowlingParkData = await _bowlingParkApiService.GetFromQrCode(qrCode);
        
        // Get or create session
        var session = _context.Sessions
            .FirstOrDefault(s => s.LocationQrCode == qrCode && s.ClosedAt == null);

        if (session != null) return session.ToSessionResponse(bowlingParkData);
        
        
        _logger.LogInformation("Creating new session for qrCode: {qrCode}", qrCode);
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
        await _context.SaveChangesAsync();

        return session.ToSessionResponse(bowlingParkData);
    }

    public PaymentActualisationResponse ActualisePayment(string sessionId, PaymentActualisationRequest model)
    {
        _logger.LogInformation("Actualising payment {paymentId} for session: {sessionId}", model.PaymentId, sessionId);
        
        var session = _context.Sessions
            .FirstOrDefault(s => s.Id == sessionId && s.ClosedAt == null)
            ?? throw new AppException("Session not found", 404);

        session.CurrentlyPaid += model.Amount;
        session.ActuallyProcessingPayment = false;
        
        _context.SaveChanges();

        // Notify
        _notificationService.SendNotificationToSession(session.LocationQrCode, 
            $"{model.Amount} € paid, total: {session.CurrentlyPaid} / {session.TotalPrice} €");
        
        return new PaymentActualisationResponse(model.PaymentId, session.CurrentlyPaid);
    }
    
    public async Task<PaymentResponse> AttemptPayment(string qrCode, PaymentRequest model, string UserId)
    {
        var session = _context.Sessions
                          .FirstOrDefault(s => s.LocationQrCode == qrCode && s.ClosedAt == null)
                      ?? throw new AppException("Session not found", 404);

        // Check if payment is not already processing
        if (session.ActuallyProcessingPayment)
            throw new AppException("Payment already processing", 400);

        var amountToPay = await ProcessPaymentPartType(model.PaymentPartType, model.Amount, session, UserId);

        var response = await _paymentApiService.SendPaymentAttemptAsync(new PaymentApiRequest(
            model.PaymentType,
            session.Id,
            UserId,
            amountToPay,
            model.CreditCardInfos
        ));

        session.ActuallyProcessingPayment = true;
        _context.Update(session);
        await _context.SaveChangesAsync();

        return new PaymentResponse(response.Id, response.PaymentState);
    }

    public async Task<OrderResponse> Order(string qrCode, OrderRequest model)
    {
        var session = _context.Sessions
            .FirstOrDefault(s => s.LocationQrCode == qrCode && s.ClosedAt == null)
            ?? throw new AppException("Session not found", 404);
        
        // Send order to OrderApi
        var response = await _orderApiService.SendOrderAsync(
            new OrderApiRequest(
                model.CustomerInfos, 
                qrCode, 
                model.ProductCommands
        ));
        
        // Add order to session
        session.Orders.Add(response.Id);
        session.TotalPrice += response.TotalPrice;
        _context.Update(session);
        await _context.SaveChangesAsync();
        
        return response;
    }
    
    public async Task<SessionResponse> TryCloseSession(string qrCode)
    {
        var session = _context.Sessions
            .FirstOrDefault(s => s.LocationQrCode == qrCode && s.ClosedAt == null)
            ?? throw new AppException("Session not found", 404);
        
        if (session.ActuallyProcessingPayment)
            throw new AppException("Payment is processing", 400);

        if (Math.Abs(session.TotalPrice - session.CurrentlyPaid) > 0.01)
            throw new AppException("Not all money paid", 400);
        
        session.ClosedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        _context.Update(session);
        await _context.SaveChangesAsync();
        
        // orders
        if (session.Orders.Count != 0)
        {
            await _orderApiService.MarkOrdersAsPaidAsync(session.Orders);
        }
        
        return session.ToSessionResponse(await _bowlingParkApiService.GetFromQrCode(qrCode));
    }

    public Task SetAmountManually(string qrCode, PaymentManualRequest model)
    {
        var session = _context.Sessions
            .FirstOrDefault(s => s.LocationQrCode == qrCode && s.ClosedAt == null)
            ?? throw new AppException("Session not found", 404);
        
        if (session.ActuallyProcessingPayment)
            throw new AppException("Payment is processing", 400);
        
        session.TotalPrice = model.Amount;
        _context.Update(session);
        return _context.SaveChangesAsync();
    }

    private async Task<float> ProcessPaymentPartType(string paymentPartType, float? amount, Session session, string userId)
    {
        switch (paymentPartType.ToUpper())
        {
            case "TOTAL":
                var amountToPay = session.TotalPrice - session.CurrentlyPaid;
                _logger.LogInformation("Try processing TOTAL payment for session {sessionId}, amount: {amount}", session.Id, amountToPay);
                
                return amountToPay;
            
            case "AMOUNT":
                _logger.LogInformation("Try processing AMOUNT payment for session {sessionId}, amount: {amount}", session.Id, amount);
                if (!amount.HasValue)
                    throw new AppException("Amount is required for AMOUNT paymentPartType", 400);
                
                if (session.TotalPrice - session.CurrentlyPaid < amount)
                    throw new AppException($"Too much money, {session.TotalPrice - session.CurrentlyPaid} < {amount}", 400);

                return amount.Value;
            
            case "HIMSELF":
                _logger.LogInformation("Try processing HIMSELF payment for session {sessionId}", session.Id);
                // Get user orders
                var userOrders = await _orderApiService.GetUnpaidOrdersByUserAsync(userId);
                
                // total price of user orders
                var userTotalPrice = userOrders.Sum(order => order.TotalPrice);
                
                if (session.TotalPrice - session.CurrentlyPaid < userTotalPrice)
                    throw new AppException($"Too much money, {session.TotalPrice - session.CurrentlyPaid} < {userTotalPrice}", 400);

                return userTotalPrice;
            default:
                throw new AppException("Invalid paymentPartType", 400);
        }
    }
}