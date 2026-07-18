namespace MedicalAppointment.Api.Interfaces;

public interface IUserValidate
{
    Task<Guid> GetDoctorForAppointment();
    Task<bool> AppointmentTimeAvailable(Guid doctorId, DateTime date);
    Task<bool> GetDaysAppointment(Guid appointmentId);

}