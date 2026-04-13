using BlazorUI.Interfaces;
using BlazorUI.Models;
using System.Net.Http.Json;
using static BlazorUI.Services.Typed_clients;

namespace BlazorUI.Services
{
    public class UserServices : IUserServices
    {
        private readonly HttpClient _httpClient;

        public UserServices(ProtectedApiClient protectedClient)
        {
            _httpClient = protectedClient.Http;
        }
        public async Task<ServiceResponse<User>> GetUserByIdAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<User>>($"api/Users/{userId}");
        }
        public async Task<ServiceResponse<User>> GetMyUserAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<User>>($"api/Users/me");

        }
        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<List<User>>>("api/Users");
        }
        public async Task<ServiceResponse<object>> AssignRoleAsync(string userId, string role)
        {
            var dto = new AssignRole
            {
                UserId = userId,
                Role = role
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("api/users/assign-role", dto);
            // Check if the response indicates success
            var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<object>>();
            // If the response is not successful, create a new ServiceResponse with an error message
            return result ?? new ServiceResponse<object> { Success = false, Message = "No response from server." };
        }
    }
}