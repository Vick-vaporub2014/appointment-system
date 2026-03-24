namespace BlazorUI.Models
{
    public class CreateAppointment
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public string? Notes { get; set; }
    }
}
