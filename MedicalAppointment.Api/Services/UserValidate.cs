using Dapper;
using MedicalAppointment.Api.Interfaces;

namespace MedicalAppointment.Api.Services;

public class UserValidate : IUserValidate
{
    private readonly IConnectionService _connectionService;

    public UserValidate(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    
    public async Task<Guid> GetDoctorForAppointment()
    {
        const string queryGetDoctors = """
                                           SELECT id
                                           FROM users
                                           WHERE role_id = 'c8bfb0f8-96c1-468b-a4b4-f3e06ff5e61f';
                                       """;

        var idsDoctors = await _connectionService.connection.QueryAsync<Guid>(queryGetDoctors);

        foreach (var idDoctor in idsDoctors)
        {
            const string sql = """
                                   SELECT COUNT(*)
                                   FROM appointment
                                   WHERE doctor_id = @DoctorId;
                               """;

            var totalAppointments = await _connectionService.connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    DoctorId = idDoctor
                });

            if (totalAppointments >= 5)
                continue;

            return idDoctor;
        }

        throw new Exception("No doctors available.");
    }
    
    public async Task<bool> AppointmentTimeAvailable(Guid doctorId, DateTime date)
    {
        const string sql = """
                           SELECT EXISTS
                           (
                               SELECT 1
                               FROM appointment
                               WHERE doctor_id = @DoctorId
                                 AND date BETWEEN @Date - INTERVAL '1 hour'
                                              AND @Date + INTERVAL '1 hour'
                           );
                           """;

        bool exists = await _connectionService.connection.ExecuteScalarAsync<bool>(
            sql,
            new
            {
                DoctorId = doctorId,
                Date = date
            });

        return !exists;
    }

    public async Task<bool> GetDaysAppointment(Guid appointmentId)
    {
        const string sql = """
                           SELECT date
                           FROM appointment
                           WHERE id = @AppointmentId;
                           """;

        var appointmentDate = await _connectionService.connection.QueryFirstOrDefaultAsync<DateTime>(
            sql,
            new
            {
                AppointmentId = appointmentId
            });

        return (appointmentDate - DateTime.UtcNow).TotalDays < 2;
    }
}