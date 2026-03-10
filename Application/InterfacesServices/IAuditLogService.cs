using Application.DTOs;
using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfacesServices
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string userId, string action, string details);
        Task<List<AuditLogDTO>> GetLogsByUserAsync(string userId);
        Task<List<AuditLogDTO>> GetAllLogsAsync();

    }
}
