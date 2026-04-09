using Application.DTOs;
using Application.InterfacesRepo;
using Application.InterfacesServices;
using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogService(IAuditLogRepository repository)
        {
            _repository = repository;
        }
        public async Task LogActionAsync(string userId, string action, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                DateTime = DateTime.UtcNow,
                Details = details

            };

            await _repository.AddAsync(log);
        }
        public async Task<List<AuditLogDTO>> GetLogsByUserAsync(string userId)
        {
            var logs = await _repository.GetByUserIdAsync(userId);
            return logs.Select(l => new AuditLogDTO
            {
                
                UserId = l.UserId,
                Action = l.Action,
                DateTime = l.DateTime,
                Details = l.Details
                
            }).ToList();
        }

        public async Task<List<AuditLogDTO>> GetAllLogsAsync()
        {
            var logs = await _repository.GetAllAsync();
            return logs.Select(l => new AuditLogDTO
            {
                
                UserId = l.UserId,
                Action = l.Action,
                DateTime = l.DateTime
            }).ToList();
        }

    }
}
