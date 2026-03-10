using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AppointmentsDTO
    {
        public int AppointmentId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public UserDTO User { get; set; }
    }
}
