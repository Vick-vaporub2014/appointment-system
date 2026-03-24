namespace BlazorUI.Models
{
    public class UpdateAppointment
    {
        public int AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
