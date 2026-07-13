using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Interfaces;

public interface IAdminService
{
    Task<Results<string, int>> CreateUser(User user);
    Task<Results<User, int>> GetUserById(Guid id);
    Task<Results<string, int>> UpdateUser(User user);
    Task<Results<string, int>> DeactivateUser(Guid id);
    Task<Results<(ICollection<User>, int), int>> GetAllUsers(Guid roleId);
    Task<Results<string, int>> ActivateUser(Guid id);
}