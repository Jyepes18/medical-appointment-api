namespace MedicalAppointment.Api.Models;

public class Report
{
    public Guid Id { get; set; }

    public Guid AppointmentId { get; set; }

    public string UrlReport { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}