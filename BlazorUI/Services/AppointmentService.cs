using BlazorUI.Interfaces;
using BlazorUI.Models;
using System.Net.Http.Json;

namespace BlazorUI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _httpClient;

        public AppointmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ServiceResponse<List<Appointment>>> GetAllAppointmentAsync()
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>("api/appointment");
        }
        public async Task<ServiceResponse<Appointment>> CreateAppointmentAsync(CreateAppointment dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/appointment", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
        }
        public async Task<ServiceResponse<Appointment>> UpdateAppointmentStatusAsync(UpdateAppointment dto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/appointment", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<Appointment>>();
        }
        public async Task<ServiceResponse<List<Appointment>>> GetAppointmentByUserAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Appointment>>>($"api/appointment/user/{userId}");
        }
        public async Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id, string userId)
        {
            var response = await _httpClient.DeleteAsync($"api/appointment/{id}?userId={userId}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
