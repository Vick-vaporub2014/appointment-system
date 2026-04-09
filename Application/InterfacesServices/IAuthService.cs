using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.DTOs.AuthDTOs;

namespace Application.InterfacesServices
{
    public interface IAuthService
    {
        Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO dto);
        Task<ServiceResponse<string>> RegisterAsync(RegisterDTO dto);
        Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(string refreshToken);

    }
}
