using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AuditLogDTO
    {
        public Guid AuditLogId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
        public string Details { get; set; }
    }
}
