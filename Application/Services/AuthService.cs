using Application.InterfacesRepo;
using Application.InterfacesServices;

using Domain.Enitities;
using Domain.Enums;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using static Application.DTOs.AuthDTOs;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager; //Services for managing user accounts (Gestiona los usarios de la tabla)
        private readonly SignInManager<ApplicationUser> _signInManager; //Services for handling user sign-in operations (Gestiona el inicio de sesion de los usuarios)
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Patient);
            }
            if (!result.Succeeded)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User registered falied",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            return new ServiceResponse<string>
            {
                Success = true,
                Message = "User registered successfully",
                Data = user.Id
            };

        }
        public async Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) { 
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Invalid email"
                };
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Invalid password"
                };
            }
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenGenerator.GenerateToken(user,roles);
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddMinutes(10)
            };  
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new ServiceResponse<TokenDTO>
            {
                Success = true,
                Message = "Login successful",
                Data = new TokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token
                }
            };

        }
        public async Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.Expires < DateTime.UtcNow || storedToken.Revoked != null)
            {
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token "
                };
            }
            var user = await _userManager.FindByIdAsync(storedToken.UserId);
             var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtTokenGenerator.GenerateToken(user, roles);

            return new ServiceResponse<TokenDTO>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = new TokenDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = storedToken.Token
                }
            };
        }


    }
}
