using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enitities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
        public string Details { get; set; }
        
    }
}
