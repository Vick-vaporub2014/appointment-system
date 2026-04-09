using BlazorUI.Interfaces;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorUI.Services
{
    // This class is a custom message handler that intercepts HTTP requests and adds the authentication token to the request headers. It also handles token refresh logic when a 401 Unauthorized response is received.
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly IAuthServices _authServices;
        
        public AuthMessageHandler(IAuthServices authServices)
        {
            _authServices = authServices;
          
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authServices.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = await _authServices.GetRefreshTokenAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshResult = await _authServices.RefreshTokenAsync(refreshToken);
                    if (refreshResult.Success)
                    {
                        await _authServices.SaveTokensAsync(refreshResult.Data);       
                     

                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.Data.AccessToken);
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
            }
            return response;
        }
    }
}
