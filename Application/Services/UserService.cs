using Application.DTOs;
using Application.InterfacesRepo;
using Application.InterfacesServices;
using Domain.Enitities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditLogService;

        public UserService(IUserRepository userRepository, IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _auditLogService = auditLogService;
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
                Data = user,
                Message = "User retrieved successfully"
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
                    Message = "User not found",
                    ErrorType = "NotFound"
                };
            }
            return new ServiceResponse<UserDTO>
            {
                Success = true,
                Data = user,
                Message = "User retrieved successfully"
            };
        }
        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync()
        {
            var user= await _userRepository.GetAllAsync();
            if (user == null || !user.Any())
            {
                return new ServiceResponse<List<UserDTO>>
                {
                    Success = false,
                    Message = "No users registered yet",
                    ErrorType = "NotFound"
                };
            }
            return new ServiceResponse<List<UserDTO>>
            {
                Success = true,
                Data = user,
                Message = "Users retrieved successfully"
            };
        }
        public async Task<ServiceResponse<object>> AssignRoleAsync(string userId, string role)
        {
            if (role != Roles.Admin && role != Roles.Patient && role != Roles.Doctor)
                return new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Invalid role",
                    ErrorType = "Validation"
                };
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ServiceResponse<object>
                {
                    Success = false,
                    Message = "User not found",
                    ErrorType = "NotFound"
                };
            }

            await _userRepository.AssignRoleAsync(userId, role);
            await _auditLogService.LogActionAsync("Assign role", $"Assign Role {role}", $"user {userId} now has the role {role}");

            return new ServiceResponse<object>
            {
                Success = true,
                Message = "Role assigned successfully"
            };
        }



    }
}
