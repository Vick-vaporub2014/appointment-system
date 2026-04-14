using BlazorUI.Models;
using BlazorUI.Services;

namespace BlazorUI.Interfaces
{
    public interface IUserServices
    {
        Task<ServiceResponse<User>> GetUserByIdAsync(string userId);
        Task<ServiceResponse<User>> GetMyUserAsync();
        Task<ServiceResponse<List<User>>> GetAllUsersAsync();
        Task<ServiceResponse<object>> AssignRoleAsync(string userId, string role);
    }
}
