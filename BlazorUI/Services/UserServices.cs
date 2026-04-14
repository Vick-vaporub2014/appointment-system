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
            try
            {
                return await _httpClient.GetFromJsonAsync<ServiceResponse<User>>($"api/Users/{userId}");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = $"Error retrieving user: {ex.Message}",
                    ErrorType = "ClientError"
                };
            }
        }
        public async Task<ServiceResponse<User>> GetMyUserAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ServiceResponse<User>>("api/Users/me");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = $"Error retrieving user: {ex.Message}",
                    ErrorType = "ClientError"
                };
            }


        }
        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ServiceResponse<List<User>>>("api/Users");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<User>>
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}",
                    ErrorType = "ClientError"
                };
            }
        }
        public async Task<ServiceResponse<object>> AssignRoleAsync(string userId, string role)
        {
            try
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
            catch (Exception ex)
            {
                return new ServiceResponse<object>
                {
                    Success = false,
                    Message = $"Error assigning role: {ex.Message}",
                    ErrorType = "ClientError"
                };
            }
        }
    }
}