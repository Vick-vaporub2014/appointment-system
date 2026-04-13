using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfacesRepo
{
    public interface IUserRepository
    {
        Task<UserDTO> GetByIdAsync(string userId);
        Task<UserDTO> GetMyUserAsync(string userId);
        Task<List<UserDTO>> GetAllAsync();
        Task AssignRoleAsync(string userId, string role);
    }
}
