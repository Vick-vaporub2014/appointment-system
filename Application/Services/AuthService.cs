using Application.InterfacesRepo;
using Application.InterfacesServices;

using Domain.Enitities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using static Application.DTOs.AuthDTOs;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenGenerator jwtTokenGenerator, 
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }
        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                var newUser = new User
                {
                    Email = dto.Email,
                    Name = dto.UserName,
                    Role = Roles.Patient
                };

                var result = await _userRepository.CreateUserAsync(newUser, dto.Password);

                if (!result)
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "User registration failed. Check security policies or duplicate emails."
                    };
                }
                return new ServiceResponse<string>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = newUser.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error in Register");
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Unexpected error: " + ex.Message
                };
            }

        }
        
        public async Task<ServiceResponse<TokenDTO>> LoginAsync(LoginDTO dto)
        {
            try { 
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null) { 
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }
            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }
            var roles = new List<string> {user.Role };
            var accessToken = _jwtTokenGenerator.GenerateToken(user,roles);
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(1)
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

        }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Unexpected error: " + ex.Message
                };
            }
        }
        public async Task<ServiceResponse<TokenDTO>> RefreshTokenAsync(string refreshToken)
        {
            try
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
                var user = await _userRepository.GetByIdAsync(storedToken.UserId);
                if (user == null)
                {
                    return new ServiceResponse<TokenDTO>
                    {
                        Success = false,
                        Message = "User associated with the token not found"
                    };
                }
                var roles = new List<string> {user.Role };
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
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error in Login");
                return new ServiceResponse<TokenDTO>
                {
                    Success = false,
                    Message = "Unexpected error: " + ex.Message
                };
            }

        }
    }
}
