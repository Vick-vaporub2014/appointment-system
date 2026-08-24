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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId) =>
            await _context.Appointments.Where(a => a.UserId == userId).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await _context.Appointments.AsNoTracking() //AsNoTracking() is used to improve performance when you don't need to track changes to the entities
                                        .ToListAsync();

        public async Task AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
   
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                
            }
        }
    }
}
