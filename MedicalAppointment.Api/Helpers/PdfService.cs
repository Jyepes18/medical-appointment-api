using Amazon.S3;
using Amazon.S3.Model;
using Dapper;
using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicalAppointment.Api.Helpers;

public class PdfService : IPdfService
{
    private readonly IAmazonS3 _client;
    private readonly RailwayBucketOptions _options;
    private readonly IConnectionService _connectionService;

    public PdfService(IAmazonS3 client, IOptions<RailwayBucketOptions> options, IConnectionService connectionService)
    {
        _client = client;
        _options = options.Value;
        _connectionService = connectionService;
    }
    
    public async Task<Results<byte[], int>> CreatePDF(VitalSign vitalSign)
    {
        try
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Medical Record")
                        .FontSize(24)
                        .Bold();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(8);

                            column.Item().Text("Vital Signs")
                                .Bold()
                                .FontSize(18);

                            column.Item().Text($"Weight: {vitalSign.Weight} kg");
                            column.Item().Text($"Height: {vitalSign.Height} m");
                            column.Item().Text($"BMI: {vitalSign.Imc}");
                            column.Item().Text($"Temperature: {vitalSign.Temperature} °C");
                            column.Item().Text($"Blood Pressure: {vitalSign.BloodPressure}");
                            column.Item().Text($"Heart Rate: {vitalSign.HeartRate} bpm");
                            column.Item().Text($"Respiratory Rate: {vitalSign.RespiratoryRate}");
                            column.Item().Text($"Oxygen Saturation: {vitalSign.OxygenSaturation}%");
                            
                            
                            column.Item().Text("Medical Recors")
                                .Bold()
                                .FontSize(18);

                            column.Item().Text(vitalSign.MedicalNotes);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Medical Appointment API");
                });
            }).GeneratePdf();

            return Results<byte[], int>.Success(pdfBytes,201);
        }
        catch (Exception)
        {
            return Results<byte[], int>.Failure("Error generating PDF", 500);
        }
    }
    
    public async Task<string> UploadPdf(byte[] pdfBytes)
    {
        var fileName = $"{Guid.NewGuid()}.pdf";

        using var stream = new MemoryStream(pdfBytes);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = "application/pdf"
        };

        await _client.PutObjectAsync(request);

        return fileName;
    }
    
    public async Task<Results<(byte[] File, string FileName), int>> DownloadReport(Guid reportId)
    {
        const string sql = @"
            SELECT url_report
            FROM public.report
            WHERE id = @ReportId;
        ";

        var fileName = await _connectionService.connection.QueryFirstOrDefaultAsync<string>(
            sql,
            new { ReportId = reportId });

        if (string.IsNullOrWhiteSpace(fileName))
            return Results<(byte[] File, string FileName), int>.Failure("Error to download file", 404);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileName
            };

            using var response = await _client.GetObjectAsync(request);
            using var memory = new MemoryStream();

            await response.ResponseStream.CopyToAsync(memory);

            return Results<(byte[] File, string FileName), int>.Success(
                (memory.ToArray(), fileName),
                200);
        }
        catch (AmazonS3Exception)
        {
            return Results<(byte[] File, string FileName), int>.Failure("Error to download file", 404);
        }
        catch (Exception)
        {
            return Results<(byte[] File, string FileName), int>.Failure("Internal server error", 500);
        }
    }
}