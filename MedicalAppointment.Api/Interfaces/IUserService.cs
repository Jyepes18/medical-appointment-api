using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Interfaces;

public interface IUserService
{
    Task<Results<string, int>> CreateNewAppoiment(Guid userId, DateTime dateAppoiment);
    Task<Results<string, int>> CancelAppoiment(Guid appoinmentId);
    Task<Results<(ICollection<Appointment>, int), int>> GetAppointmentsByUser(Guid userId);
    Task<Results<(ICollection<Report>, int), int>> GetReportsByUser(Guid userId, int page, int pageSize);
}