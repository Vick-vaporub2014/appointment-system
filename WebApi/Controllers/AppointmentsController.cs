using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //The route is set to "api/appointments" based on the controller name
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO request)
        {
            var result = await _appointmentService.CreateAppointmentAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = Roles.Admin +","+Roles.Doctor)]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateAppointmentStatus([FromBody] UpdateAppointmentDTO request)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(request);
            if (!result.Success)
            {
                if (result.ErrorType == "NotFound") return NotFound(result);
                if (result.ErrorType == "Unauthorized") return Unauthorized(result);

                return BadRequest(result);
            }
            return Ok(result);
        }
        //[Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _appointmentService.GetAllAppointmentAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAppointmentsByUser(string userId)
        {
            var result = await _appointmentService.GetAppointmentByUserAsync(userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize(Roles = Roles.Admin + "," + Roles.Doctor)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result.Success)
            {
                if (result.ErrorType == "NotFound") return NotFound(result);
                if (result.ErrorType == "Unauthorized") return Unauthorized(result);
                return BadRequest(result);
            }
            return Ok(result);

        }
    }
}
