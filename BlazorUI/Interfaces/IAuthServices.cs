
using BlazorUI.Services;
using static BlazorUI.Models.Auth;

namespace BlazorUI.Interfaces
{
    public interface IAuthServices
    {
        Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO dto);
        Task<ServiceResponse<string>> RegisterAsync(RegisterDTO dto);
        Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(string refreshToken);
        Task<string?> GetTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task SaveTokensAsync(TokenDTO data);
        Task LogoutAsync();
    }
}
