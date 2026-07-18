using MedicalAppointment.Api.Models;

namespace MedicalAppointment.Api.Interfaces;

public interface IPdfService
{
    Task<Results<byte[], int>> CreatePDF(VitalSign vitalSign);
    Task<string> UploadPdf(byte[] pdfBytes);
    Task<Results<(byte[] File, string FileName), int>> DownloadReport(Guid reportId);
}