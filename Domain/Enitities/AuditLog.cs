

namespace Domain.Enitities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }
        
    }
}
