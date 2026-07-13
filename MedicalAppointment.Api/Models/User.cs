namespace MedicalAppointment.Api.Models;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string Document { get; set; }

    public string Gender { get; set; }

    public DateTime DateBirth { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public bool Status { get; set; }

    public Guid RoleId { get; set; }
    
    public string? Role { get; }
}