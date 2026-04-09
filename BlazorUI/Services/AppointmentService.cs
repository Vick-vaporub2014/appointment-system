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
            return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>("api/Appointments");
        }
        public async Task<ServiceResponse<Appointment>> CreateAppointmentAsync(CreateAppointment dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Appointments", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
        }
        public async Task<ServiceResponse<Appointment>> UpdateAppointmentStatusAsync(UpdateAppointment dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Appointments/status", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
        }
        public async Task<ServiceResponse<List<Appointment>>> GetAppointmentByUserAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>($"api/Appointments/user/{userId}");
        }
        public async Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Appointments/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
