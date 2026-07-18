using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("[Controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    [Route("get-all-apointments/{doctorId}")]
    public async Task<IActionResult> GetAll([FromRoute] Guid doctorId)
    {
        var result = await _doctorService.GetAllAppointments(doctorId);
        return Ok(new {Data = result.Value.Item1, Total = result.Value.total, result.Status, result.IsSuccess, result.Error});
    }

    [HttpPost]
    [Route("insert-appointment")]
    public async Task<IActionResult> Insert([FromBody] VitalSign vitalSign)
    {
        var result = await _doctorService.CreateNewRegister(vitalSign, vitalSign.MedicalNotes, vitalSign.AppointmentId,
            vitalSign.UserId);

        return Ok(result);
    }
}