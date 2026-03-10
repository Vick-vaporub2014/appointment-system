using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateAppointmentDTO
    {
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public string Notes { get; set; }

    }
}
