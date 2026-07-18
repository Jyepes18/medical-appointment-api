using System.Data.Common;
using Dapper;
using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Services;

public class DoctorService : IDoctorService
{
    private readonly IConnectionService _connectionService;
    private readonly IPdfService _pdfService;

    public DoctorService(IConnectionService connectionService, IPdfService pdfService)
    {
        _connectionService = connectionService;
        _pdfService = pdfService;
    }
    
    public async Task<Results<(ICollection<AppointmentDoctor>, int total), int>> GetAllAppointments(Guid doctorId)
    {
        const string sql = @"
            SELECT
                a.doctor_id AS DoctorId,
                a.id AS AppointmentId,
                a.user_id as PatientId,
                CONCAT_WS(' ', u.name, u.last_name) AS FullNamePatient,
                CONCAT_WS(' ', d.name, d.last_name) AS FullNameDoctor,
                TO_CHAR(a.date, 'DD/MM/YYYY') AS DateAppointment
            FROM appointment a
            INNER JOIN users u
                ON a.user_id = u.id
            INNER JOIN users d
                ON a.doctor_id = d.id
            WHERE a.doctor_id = @DoctorId;

            SELECT COUNT(*)
            FROM appointment
            WHERE doctor_id = @DoctorId;
        ";

        var multi = await _connectionService.connection.QueryMultipleAsync(sql, new
        {
            DoctorId = doctorId
        });

        var appointments = (await multi.ReadAsync<AppointmentDoctor>()).ToList();
        var total = await multi.ReadFirstAsync<int>();

        return Results<(ICollection<AppointmentDoctor>, int total), int>.Success((appointments, total), 200);
    }

    public async Task<Results<string, int>> CreateNewRegister(VitalSign vitalSign, string medicalRecord, Guid appointmentId, Guid userId)
    {
        await using var transaction = await _connectionService.connection.BeginTransactionAsync();

        Guid vitalSignId = await InsertVitalSign(vitalSign, transaction);
        if (vitalSignId == null)
        {
            await transaction.RollbackAsync();
            return Results<string, int>.Failure("we can't insert vital sign", 404);
        }

        bool insertMedicalRecord =
            await InsertMedicalRecord(appointmentId, userId, vitalSignId, medicalRecord, transaction);
        if (!insertMedicalRecord)
        {
            await transaction.RollbackAsync();
            return Results<string, int>.Failure("we can't insert medical record", 404);
        }
        
        var pdfResult = await _pdfService.CreatePDF(vitalSign);

        if (!pdfResult.IsSuccess)
        {
            await transaction.RollbackAsync();
            return Results<string, int>.Failure("Error generating PDF", 500);
        }

        string insertPdf = await _pdfService.UploadPdf(pdfResult.Value);
        if (string.IsNullOrEmpty(insertPdf))
        {
            await transaction.RollbackAsync();
            return Results<string, int>.Failure("Error insert PDF", 500);
        }

        bool report = await InsertReport(vitalSign.AppointmentId, insertPdf, transaction);
        if (!report)
        {
            await transaction.RollbackAsync();
            return Results<string, int>.Failure("we can't insert report", 404);
        }
        
        await transaction.CommitAsync();
        return Results<string, int>.Success("Appointment insert success", 201);
    }
    
    public async Task<Guid> InsertVitalSign(VitalSign vitalSign, DbTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO vital_sign
                (
                    weight,
                    height,
                    imc,
                    temperature,
                    blood_pressure,
                    heart_rate,
                    respiratory_rate,
                    oxygen_saturation
                )
                VALUES
                (
                    @Weight,
                    @Height,
                    @Imc,
                    @Temperature,
                    @BloodPressure,
                    @HeartRate,
                    @RespiratoryRate,
                    @OxygenSaturation
                )
                RETURNING id;
            ";

            var result = await _connectionService.connection.ExecuteScalarAsync<Guid>(sql, vitalSign, transaction);
            
            return result;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error to insert on vital_sign ", e);
        }
    }
    
    public async Task<bool> InsertMedicalRecord(Guid appointmentId, Guid userId, Guid vitalSignId, string medicalRecord, DbTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO medical_record
                (
                    user_id,
                    appointment_id,
                    vital_sign_id,
                    medical_notes
                )
                VALUES
                (
                    @UserId,
                    @AppointmentId,
                    @VitalSignId,
                    @MedicalNotes
                );
            ";

            var result = await _connectionService.connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                AppointmentId = appointmentId,
                VitalSignId = vitalSignId,
                MedicalNotes = medicalRecord
            },transaction);

            return result > 0;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error to insert on medical_record ", ex);
        }
    }
    
    public async Task<bool> InsertReport(Guid appointmentId, string urlReport, DbTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO public.report
                (
                    appointment_id,
                    url_report,
                    created_at
                )
                VALUES
                (
                    @AppointmentId,
                    @UrlReport,
                    @CreatedAt
                );
            ";

            var result = await _connectionService.connection.ExecuteAsync(sql, new
            {

                AppointmentId = appointmentId,
                UrlReport = urlReport,
                CreatedAt = DateTime.UtcNow
            },transaction);

            return result > 0;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error to insert on report ", ex);
        }
    }
    
    
    
}