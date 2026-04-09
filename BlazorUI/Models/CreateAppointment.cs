namespace BlazorUI.Models
{
    public class CreateAppointment
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }

        //public string DateTimeString { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
