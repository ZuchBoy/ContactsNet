using System.Net.Http.Headers;

namespace ContactsFront.Services;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly AuthService _auth;

    public AuthMessageHandler(AuthService auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Sends an HTTP request with an optional bearer token for authentication.Adds the bearer token to outgoing HTTP requests if available.
    /// </summary>
    /// <remarks>If a bearer token is available, it is added to the request's Authorization header before
    /// sending the request. If no token is available, the request is sent without an Authorization header.</remarks>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message received
    /// from the server.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _auth.GetTokenSafeAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
