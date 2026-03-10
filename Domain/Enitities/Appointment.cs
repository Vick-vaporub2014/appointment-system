using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enitities
{
    
    public class Appointment
    {
        public int AppointmentId { get; private set; }
        public string UserId { get; private set; } //Referenes to AspNet Identity User
        public DateTime DateTime { get; private set; }
        public string Status { get; private set; }
        public string Notes { get; private set; }

        public Appointment() { } //EF Core requires a parameterless constructor
        //Validations Constructor
        public Appointment(string userId, DateTime dateTime, string status, string notes)
        {
            ValidateUserId(userId);
            ValidateDateTime(dateTime);
            ValidateStatus(status);

            UserId = userId;
            DateTime = dateTime;
            Status = status;
            Notes = notes?.Trim();
        }
        //Method to update the status of the appointment
        public void UpdateStatus(string status, string notes)
        {
            ValidateStatus(status);
            Status = status;
            Notes = notes?.Trim();
        }
        //Validation Methods
        private void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("El UserId no puede estar vacio.", nameof(userId));
        }
        private void ValidateDateTime(DateTime dateTime)
        {
            if (dateTime < DateTime.Now)
                throw new ArgumentException("La fecha de la cita no puede ser en el pasado.", nameof(dateTime));
        }
        private void ValidateStatus(string status)
        {
            var validStatuses = new[] { "pendiente", "aceptada", "cancelada" };
            if (!validStatuses.Contains(status.ToLower()))
                throw new ArgumentException("El estado de la cita no es valido.", nameof(status));
        }
    }
}
