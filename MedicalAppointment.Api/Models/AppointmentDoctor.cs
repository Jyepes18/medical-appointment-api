namespace MedicalAppointment.Api.Models;

public class AppointmentDoctor
{
    public Guid DoctorId { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string FullNamePatient { get; set; }
    public string DateAppointment { get; set; }
}