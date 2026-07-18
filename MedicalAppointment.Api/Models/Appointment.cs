namespace MedicalAppointment.Api.Models;

public class Appointment
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string UserLastName { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;

    public string DoctorLastName { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}