using BlazorUI.Interfaces;
using BlazorUI.Pages;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorUI.Services
{
    // This class is a custom message handler that intercepts HTTP requests and adds the authentication token to the request headers. It also handles token refresh logic when a 401 Unauthorized response is received.
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly IAuthServices _authServices;
        private readonly NavigationManager _navigationManager;

        public AuthMessageHandler(IAuthServices authServices, NavigationManager navigationManager)
        {
            _authServices = authServices;
            _navigationManager = navigationManager;

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authServices.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                var jwtInfo = JwtParser.ParseClaimsFromJwt(token);
                if (jwtInfo.Expiration <= DateTime.UtcNow)
                {
                    var refreshToken = await _authServices.GetRefreshTokenAsync();
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var refreshResult = await _authServices.RefreshTokenAsync(refreshToken);
                        if (refreshResult.Success)
                        {
                            await _authServices.SaveTokensAsync(refreshResult.Data);
                            token = refreshResult.Data.AccessToken;
                        }
                        else
                        {
                            await _authServices.LogoutAsync(); 
                            _navigationManager.NavigateTo("/login", forceLoad: true);
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        }
                    }
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // if the response is 401 Unauthorized, it means the token is invalid or expired, so we log out the user and redirect to the login page.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _authServices.LogoutAsync();
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }

            return response;

        }
    }
}
