using Application.DTOs;
using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<AppointmentsDTO>> CreateAppointmentAsync(CreateAppointmentDTO dto);
        Task<ServiceResponse<AppointmentsDTO>> UpdateAppointmentStatusAsync(UpdateAppointmentDTO dto);
        Task<ServiceResponse<List<AppointmentsDTO>>> GetAllAppointmentAsync();
        Task<ServiceResponse<List<AppointmentsDTO>>> GetAppointmentByUserAsync(string userId);
        Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id, string userId);

    }


}
