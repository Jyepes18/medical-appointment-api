namespace MedicalAppointment.Api.Models;

public class CreateAppointmentRequest
{
    public Guid UserId { get; set; }
    public DateTime DateAppointment { get; set; }
}