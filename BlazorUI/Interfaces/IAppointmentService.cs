using BlazorUI.Models;
using BlazorUI.Services;

namespace BlazorUI.Interfaces
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<Appointment>> CreateAppointmentAsync(CreateAppointment dto);
        Task<ServiceResponse<Appointment>> UpdateAppointmentStatusAsync(UpdateAppointment dto);
        Task<ServiceResponse<List<Appointment>>> GetAllAppointmentAsync();
        Task<ServiceResponse<List<Appointment>>> GetAppointmentByUserAsync(string userId);
        Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id, string userId);
    }
}
