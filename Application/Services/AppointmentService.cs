using Application.DTOs;
using Application.Interfaces;
using Application.InterfacesRepo;
using Application.InterfacesServices;
using Application.UnitOfWork;
using Domain.Enitities;


namespace Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;


        public AppointmentService(IAppointmentRepository repository, IAuditLogService auditLogService, IUserService userService, IUnitOfWork unitOfWork )
        {
            _repository = repository;
            _auditLogService = auditLogService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse<AppointmentsDTO>> CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            var existingAppointments = await _repository.GetByUserIdAsync(dto.UserId);
            bool hasConflict = existingAppointments.Any(a =>
            dto.DateTime < a.DateTime.AddHours(1) &&
            dto.DateTime.AddHours(1) > a.DateTime);

            if (hasConflict)
            {
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = "Cannot create appointment: overlaps with another appointment within 1 hour.",
                    ErrorType = "BusinessRule"
                };
            }


            var appointment = new Appointment(
                dto.UserId,
                dto.DateTime,
                "pendiente",
                dto.Notes
            );
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _repository.AddAsync(appointment);
                await _auditLogService.LogActionAsync(dto.UserId, "Created", $"Created appointment {appointment.AppointmentId} for user {dto.UserId} at {dto.DateTime}");
                await _unitOfWork.CommitAsync();
                var userDto = await _userService.GetUserByIdAsync(appointment.UserId);
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = true,
                    Data = new AppointmentsDTO
                    {
                        AppointmentId = appointment.AppointmentId,
                        DateTime = appointment.DateTime,

                        Status = appointment.Status,
                        Notes = appointment.Notes,
                        User = userDto.Data
                    },
                    Message = "Appointment created successfully"
                };
            }
            catch (InvalidOperationException ex) {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = $"Error creating appointment: {ex.Message}",
                    ErrorType = "ServerError"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = $"Error creating appointment: {ex.Message}",
                    ErrorType = "Exception"
                };
            }
        }
        public async Task<ServiceResponse<AppointmentsDTO>> UpdateAppointmentStatusAsync(UpdateAppointmentDTO dto)
        {
            var appointment = await _repository.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
            {
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = "Appointment not found",
                    ErrorType = "NotFound"
                };
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                appointment.UpdateStatus(dto.Status, dto.Notes);
                await _repository.UpdateAsync(appointment);
                await _auditLogService.LogActionAsync(appointment.UserId, "Updated", $"Update appointment {appointment.AppointmentId} for {appointment.UserId} at {appointment.DateTime}" );
                await _unitOfWork.CommitAsync();
                var userDto = await _userService.GetUserByIdAsync(appointment.UserId);
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = true,
                    Data = new AppointmentsDTO
                    {
                        AppointmentId = appointment.AppointmentId,
                        DateTime = appointment.DateTime,
                        Status = appointment.Status,
                        Notes = appointment.Notes,
                        User = userDto.Data

                    },
                    Message = "Appointment updated successfully"
                };
            }
            catch (InvalidOperationException ex) 
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = $"Error updated appointment: {ex.Message}",
                    ErrorType = "ServerError"
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<AppointmentsDTO>
                {
                    Success = false,
                    Message = $"Error creating appointment: {ex.Message}",
                    ErrorType = "Exception"
                };
            }
        }
        public async Task<ServiceResponse<List<AppointmentsDTO>>> GetAllAppointmentAsync()
        {
            var appointments = await _repository.GetAllAsync();
            if (appointments == null || !appointments.Any())
            {
                return new ServiceResponse<List<AppointmentsDTO>>
                {
                    Success = false,
                    Message = "No appointments found",
                    ErrorType = "NotFound"
                };
            }
            var dtoList = new List<AppointmentsDTO>();
            foreach (var appointment in appointments) {
                var userDto = await _userService.GetUserByIdAsync(appointment.UserId);
                dtoList.Add(new AppointmentsDTO
                {
                    AppointmentId = appointment.AppointmentId,
                    DateTime = appointment.DateTime,
                    Status = appointment.Status,
                    Notes = appointment.Notes,
                    User = userDto.Data
                });
            }
            return new ServiceResponse<List<AppointmentsDTO>>
            {
                Success = true,
                Data = dtoList,
                Message = "Appointments retrieved successfully"
             };
        }
        public async Task<ServiceResponse<List<AppointmentsDTO>>> GetAppointmentByUserAsync(string userId)
        {
            var appointments = await _repository.GetByUserIdAsync(userId);
            if (appointments == null || !appointments.Any())
            {
                return new ServiceResponse<List<AppointmentsDTO>>
                {
                    Success = false,
                    Message = "No appointments found for this user",
                    ErrorType = "NotFound"
                };
            }
            var userDto = await _userService.GetUserByIdAsync(userId);
            var dtoList = appointments.Select(a => new AppointmentsDTO
            {
                AppointmentId = a.AppointmentId,
                DateTime = a.DateTime,
                Status = a.Status,
                Notes = a.Notes,
                User = userDto.Data
            }).ToList();
            return new ServiceResponse<List<AppointmentsDTO>>
            {
                Success = true,
                Data = dtoList,
                Message = "Appointments retrieved successfully"
             };
        }
        public async Task<ServiceResponse<bool>> DeleteAppointmentAsync(int id, string userId)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Appointment not found",
                    ErrorType = "NotFound"
                };
            }
            if (appointment.UserId != userId)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized to delete this appointment",
                    ErrorType = "Unauthorized"
                };
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _repository.DeleteAsync(id);
                await _auditLogService.LogActionAsync(userId, "Deleted", $"Deleted appointment {id} for {appointment.UserId} at {appointment.DateTime}");
                await _unitOfWork.CommitAsync();
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Appointment deleted successfully"
                };
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error Deleted appointment: {ex.Message}",
                    ErrorType = "ServerError"
                };

            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollbackAsync();
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error Deleted appointment: {ex.Message}",
                    ErrorType = "Exception"
                };
            }
        }
        }
}
