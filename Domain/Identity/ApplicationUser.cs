using Domain.Enitities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }

    }
}
