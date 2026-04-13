using Application.DTOs;
using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfacesServices
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDTO>> GetUserByIdAsync(string userId);
        Task<ServiceResponse<UserDTO>> GetMyUserAsync(string userId);
        Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync();
        Task<ServiceResponse<object>> AssignRoleAsync(string userId, string role);
    }
}
