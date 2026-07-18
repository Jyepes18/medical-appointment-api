namespace MedicalAppointment.Api.Models;

public class MedicalRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid VitalSignId { get; set; }
    public string MedicalNotes { get; set; } = string.Empty;
}