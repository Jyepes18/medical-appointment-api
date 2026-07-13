namespace MedicalAppointment.Api.Interfaces;

public interface IAdminValidation
{
    Task<bool> UserExist(string email);
    Task<bool> DocumentExist(string document);

}