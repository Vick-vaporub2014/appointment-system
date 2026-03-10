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

        public AppointmentRepository(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<Appointment> GetByIdAsync(int id) =>
            await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == id);
        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId) =>
            await _context.Appointments.Where(a => a.UserId == userId).ToListAsync();

        public async Task<IEnumerable<Appointment>> GetAllAsync() =>
            await _context.Appointments.ToListAsync();
        public async Task AddAsync (Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

        }
        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }   
    }
}
