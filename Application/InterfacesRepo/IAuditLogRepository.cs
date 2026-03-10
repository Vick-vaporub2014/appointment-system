using Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfacesRepo
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog log);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId);
        Task<IEnumerable<AuditLog>> GetAllAsync();

    }
}
