using BlazorUI.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using static BlazorUI.Models.Auth;

namespace BlazorUI.Services
{
    //AuthenticationStateProvider is use to manage the authentication state of the user in the Blazor application. It is used to determine if the user is authenticated or not and to provide the claims of the user to the application.
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _localStorage.GetItem<string>("authToken");

            if (string.IsNullOrEmpty(token))
            {
                
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        

        public Task NotifyUserAuthentication(TokenDTO tokens)
        {

            var claims = JwtParser.ParseClaimsFromJwt(tokens.AccessToken);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            return Task.CompletedTask;
        }

        public Task NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());


            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
            return Task.CompletedTask;
        }

    }
}
