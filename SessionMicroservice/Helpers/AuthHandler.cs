namespace SessionMicroservice.Helpers;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class AuthHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _tokenProvider.GetToken();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}