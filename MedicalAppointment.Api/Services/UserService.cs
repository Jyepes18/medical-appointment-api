using Dapper;
using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Services;

public class UserService : IUserService
{
    private readonly IConnectionService _connectionService;
    private readonly IUserValidate _userValidate;

    public UserService(IConnectionService connectionService, IUserValidate userValidate)
    {
        _connectionService = connectionService;
        _userValidate = userValidate;
    }

    public async Task<Results<string, int>> CreateNewAppoiment(Guid userId, DateTime dateAppoiment)
    {
        // First, validate what doctor is free (A doctor can have up to five patients)
        Guid idDoctor = await _userValidate.GetDoctorForAppointment();
        
        bool available = await _userValidate.AppointmentTimeAvailable(idDoctor, dateAppoiment);
        if (!available) return Results<string, int>.Failure("This appointment time is not available.", 400);
        
        const string generateAppoiment = """
                                             INSERT INTO appointment
                                             (
                                                 user_id,
                                                 doctor_id,
                                                 date
                                             )
                                             VALUES
                                             (
                                                @UserId,
                                                @DoctorId,
                                                @Date
                                             );
                                         """;

        var result = await _connectionService.connection.ExecuteAsync(generateAppoiment, new
        {
            UserId = userId,
            DoctorId = idDoctor,
            Date = dateAppoiment
        });

        if (result > 0) return Results<string, int>.Success("Appointment created successfully", 201);

        return Results<string, int>.Failure("Sorry we can't created the appointment", 404);
    }

    public async Task<Results<string, int>> CancelAppoiment(Guid appoinmentId)
    {
        bool validateAppoinment = await _userValidate.GetDaysAppointment(appoinmentId);

        if (validateAppoinment)
            return Results<string, int>.Failure("You can't delete this appointment.", 400);

        const string sql = """
                           DELETE FROM appointment
                           WHERE id = @AppointmentId;
                           """;

        var result = await _connectionService.connection.ExecuteAsync(sql, new
        {
            AppointmentId = appoinmentId
        });

        return result > 0
            ? Results<string, int>.Success("Appointment deleted successfully.", 200)
            : Results<string, int>.Failure("Appointment not found.", 404);
    }  
    
    public async Task<Results<(ICollection<Appointment>, int), int>> GetAppointmentsByUser(Guid userId)
    {
        const string sqlAppointments = """
                                           SELECT
                                               a.id,
                                               a.user_id AS UserId,
                                               a.doctor_id AS DoctorId,
                                               CONCAT(d.name, ' ', d.last_name) AS DoctorName,
                                               a.date::timestamp
                                           FROM appointment a
                                           INNER JOIN users d
                                               ON a.doctor_id = d.id
                                           WHERE a.user_id = @UserId;
                                       """;

        const string sqlCount = """
                                SELECT COUNT(*)
                                FROM appointment
                                WHERE user_id = @UserId;
                                """;

        var appointments = (await _connectionService.connection.QueryAsync<Appointment>(
                sqlAppointments,
                new { UserId = userId }))
            .ToList();

        var total = await _connectionService.connection.ExecuteScalarAsync<int>(
            sqlCount,
            new { UserId = userId });

        return Results<(ICollection<Appointment>, int), int>.Success(
            (appointments, total),
            200);
    }
    
    public async Task<Results<(ICollection<Report>, int), int>> GetReportsByUser(Guid userId, int page, int pageSize)
    {
        const string sql = @"
            SELECT
                r.id,
                r.appointment_id AS AppointmentId,
                r.url_report AS UrlReport,
                r.created_at AS CreatedAt
            FROM report r
            INNER JOIN appointment a
                ON a.id = r.appointment_id
            WHERE a.user_id = @UserId
            ORDER BY r.created_at DESC
            LIMIT @PageSize
            OFFSET @Offset;

            SELECT COUNT(*)
            FROM report r
            INNER JOIN appointment a
                ON a.id = r.appointment_id
            WHERE a.user_id = @UserId;
        ";

        using var multi = await _connectionService.connection.QueryMultipleAsync(sql, new
        {
            UserId = userId,
            PageSize = pageSize,
            Offset = (page - 1) * pageSize
        });

        var reports = (await multi.ReadAsync<Report>()).ToList();
        var total = await multi.ReadFirstAsync<int>();

        return Results<(ICollection<Report>, int), int>.Success((reports, total), 200);
    }
    
    
    
}