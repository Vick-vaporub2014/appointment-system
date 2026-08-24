using Application.DTOs;
using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfacesRepo
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
        Task<User?> GetMyUserAsync(string userId);
        Task<List<User>> GetAllAsync();
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> CreateUserAsync(User newUser, string password);
        Task<User> FindByEmailAsync(string email);
    }
}
