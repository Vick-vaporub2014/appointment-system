using Application.DTOs;
using Application.InterfacesRepo;
using Application.InterfacesServices;
using Domain.Enitities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<UserDTO>> GetUserByIdAsync(string userId)
        {
            
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return new ServiceResponse<UserDTO>
                {
                    Success = false,
                    Message = "User not found",
                    ErrorType = "NotFound"
                };
            }
            return new ServiceResponse<UserDTO>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = new UserDTO { 
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,


                }
                
            };
        }
        public async Task<ServiceResponse<UserDTO>> GetMyUserAsync(string userId)
        {
            var user = await _userRepository.GetMyUserAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<UserDTO>
                {
                    Success = false,
                    Message = "Profile information could not be retrieved",
                    ErrorType = "NotFound"
                };
            }
            return new ServiceResponse<UserDTO>
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = new UserDTO
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                }
                
            };
        }
        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();// Get all User entities from the repository -> List<User>
            if (users == null || !users.Any())
            {
                return new ServiceResponse<List<UserDTO>>
                {
                    Success = false,
                    Message = "No users registered yet",
                    ErrorType = "NotFound"
                };
            }
            // Map User entities to UserDTOs -> List<UserDTO>
            var usersDTO = users.Select(u => new UserDTO
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();

            return new ServiceResponse<List<UserDTO>>
            {
                Success = true,
                Data = usersDTO,
                Message = "Users retrieved successfully"
            };
        }
        public async Task<ServiceResponse<AssignRoleDTO>> AssignRoleAsync(string userId, string role)
        {
            if (role != Roles.Admin && role != Roles.Patient && role != Roles.Doctor)
                return new ServiceResponse<AssignRoleDTO>
                {
                    Success = false,
                    Message = "Invalid role",
                    ErrorType = "Validation"
                };
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<AssignRoleDTO>
                {
                    Success = false,
                    Message = "User not found",
                    ErrorType = "NotFound"
                };
            }

            var isAssigned = await _userRepository.AssignRoleAsync(userId, role);
            if (!isAssigned)
            {
                return new ServiceResponse<AssignRoleDTO>
                {
                    Success = false,
                    Message = "Failed to assign role due to an internal identity error",
                    ErrorType = "InternalError"
                };
            }
            var actorUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);// Get the ID of the user performing the action
            if (string.IsNullOrEmpty(actorUserId))
            {
                return new ServiceResponse<AssignRoleDTO>
                {
                    Success = false,
                    Message = "Unable to identify the user performing the action",
                    ErrorType = "Unauthorized"
                };
            }
            await _auditLogService.LogActionAsync(actorUserId, $"Assign Role {role}", $"user {userId} now has the role {role}");

            return new ServiceResponse<AssignRoleDTO>
            {
                Success = true,
                Message = "Role assigned successfully",
                Data = new AssignRoleDTO
                {
                    UserId = userId,
                    Role = role
                }
            };
        }



    }
}
