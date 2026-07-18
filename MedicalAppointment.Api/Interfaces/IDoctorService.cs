using System.Data.Common;
using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Interfaces;

public interface IDoctorService
{
    Task<Results<(ICollection<AppointmentDoctor>, int total), int>> GetAllAppointments(Guid doctorId);
    Task<Results<string, int>> CreateNewRegister(VitalSign vitalSign, string medicalRecord, Guid appointmentId, Guid userId);

    Task<Guid> InsertVitalSign(VitalSign vitalSign, DbTransaction transaction);

    Task<bool> InsertMedicalRecord(Guid appointmentId, Guid userId, Guid vitalSignId, string medicalRecord,
        DbTransaction transaction);
}