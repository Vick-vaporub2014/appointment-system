using Application.DTOs;
using Application.InterfacesRepo;
using Application.InterfacesServices;
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

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync()
        {
            var user= await _userRepository.GetAllAsync();
            if (user == null || !user.Any())
            {
                return new ServiceResponse<List<UserDTO>>
                {
                    Success = false,
                    Message = "No users found",
                    ErrorType = "NotFound"
                };
            }
            return new ServiceResponse<List<UserDTO>>
            {
                Success = true,
                Data = user.ToList(),
                Message = "Users retrieved successfully"
            };
        }
        public async Task AssignRoleAsync(string userId, string role)
        {
            
            if (role != Roles.Admin && role != Roles.Patient && role!= Roles.Doctor)
                throw new ArgumentException("Invalid role");

            await _userRepository.AssignRoleAsync(userId, role);
        }


    }
}
