using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdateAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }      

    }
}
