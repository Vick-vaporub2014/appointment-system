using Application.InterfacesRepo;
using Domain.Enitities;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositries
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()=>
            await _context.AuditLogs.ToListAsync();

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId) =>
            await _context.AuditLogs.Where(log => log.UserId == userId).ToListAsync();


    }
}
