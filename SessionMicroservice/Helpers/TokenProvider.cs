namespace SessionMicroservice.Helpers;

public interface ITokenProvider
{
    string GetToken();
}
public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetToken()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
        
        if (token.HasValue)
            return token.Value.ToString().Replace("Bearer ", string.Empty);
        
        return string.Empty;
    }
}