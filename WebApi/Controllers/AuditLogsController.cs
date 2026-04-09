using Application.InterfacesServices;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //The route is set to "api/auditlogs" based on the controller name
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var result = await _auditLogService.GetAllLogsAsync();
                return Ok(result);
                
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Success = false,ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetLogsByUser(string userId)
        {
            try
            {
                var result = await _auditLogService.GetLogsByUserAsync(userId);
                if (result == null || !result.Any())
                {
                    return Ok(new { Success = true, Message = $"No logs found dor user {userId}" } );
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Success = false, ex.Message });
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
