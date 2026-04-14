using BlazorUI.Interfaces;
using BlazorUI.Models;
using System.Net.Http.Json;
using static BlazorUI.Services.Typed_clients;

namespace BlazorUI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _httpClient;

        public AppointmentService(ProtectedApiClient protectedClient)
        {
            _httpClient = protectedClient.Http;
        }
        public async Task<ServiceResponse<List<Appointment>>> GetAllAppointmentAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>("api/Appointments");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Appointment>>
                {
                    Success = false,
                    Message = $"Error retrieving appointments: {ex.Message}",
                    Data = null,
                    ErrorType = "ClientError"
                };
            }
        }
        public async Task<ServiceResponse<Appointment>> CreateAppointmentAsync(CreateAppointment dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Appointments", dto);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Appointment>
                {
                    Success = false,
                    Message = $"Error retrieving appointments: {ex.Message}",
                    Data = null,
                    ErrorType = "ClientError"
                };

            }
        }
        public async Task<ServiceResponse<Appointment>> UpdateAppointmentStatusAsync(UpdateAppointment dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/Appointments/status", dto);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Appointment>
                {
                    Success = false,
                    Message = $"Error updating appointment status: {ex.Message}",
                    Data = null,
                    ErrorType = "ClientError"
                };
            }
        public async Task<ServiceResponse<List<Appointment>>> GetAppointmentByUserAsync(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>($"api/Appointments/user/{userId}");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Appointment>>
                {
                    Success = false,
                    Message = $"Error retrieving appointments: {ex.Message}",
                    Data = null,
                    ErrorType = "ClientError"
                };
            }
        }
        public async Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Appointments/{id}");
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting appointment: {ex.Message}",
                    Data = false,
                    ErrorType = "ClientError"
                };
            }
        }
    }
