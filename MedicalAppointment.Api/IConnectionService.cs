using Npgsql;

namespace MedicalAppointment.Api;

public interface IConnectionService
{
    NpgsqlConnection connection { get; }
}