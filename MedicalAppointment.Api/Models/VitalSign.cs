namespace MedicalAppointment.Api.Models;

public class VitalSign : MedicalRecord
{
    public Guid Id { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Imc { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
}