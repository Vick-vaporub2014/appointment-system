using BlazorUI.Interfaces;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using static BlazorUI.Models.Auth;
using static BlazorUI.Services.Typed_clients;

namespace BlazorUI.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorageService;
       

        public AuthServices(PublicApiClient publicClient, ILocalStorageService localStorage)
        {
            _http = publicClient.Http;
            _localStorageService = localStorage;
           
        }

        public async Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<TokenDTO>>();

            if (result.Success)
            {
                await SaveTokensAsync(result.Data);

            }

            return result;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDTO dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(string refreshToken)
        {
            var response = await _http.PostAsJsonAsync("api/auth/refresh", refreshToken);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<TokenDTO>>();
            if (result.Success)
            {
                await SaveTokensAsync(result.Data);

            }

            return result;
        }

        public Task<string?> GetTokenAsync()
        {
            var token = _localStorageService.GetItem<string>("authToken");
            return Task.FromResult(token);//I use FromResult to return a Task<string?> because the method is async and I want to return the token as a string.
        }
        public Task<string?> GetRefreshTokenAsync()
        {
            var token = _localStorageService.GetItem<string>("refreshToken");
            return Task.FromResult(token);
        }
        public Task SaveTokensAsync(TokenDTO data)
        {
            if (data != null)
            {
                _localStorageService.SetItem("authToken", data.AccessToken);
                _localStorageService.SetItem("refreshToken", data.RefreshToken);
            }

            return Task.CompletedTask;
        }
        public Task LogoutAsync()
        {
            _localStorageService.RemoveItem("authToken");
            _localStorageService.RemoveItem("refreshToken");

          
            // The same as FromResult but for void methods, it returns a completed task.
            return Task.CompletedTask;
        }

    }
}
