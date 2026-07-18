using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPdfService _pdfService;

    public UserController(IUserService userService, IPdfService pdfService)
    {
        _userService = userService;
        _pdfService = pdfService;
    }

    [HttpPost]
    [Route("create-appointment")]
    public async Task<IActionResult> CreateNewAppointment([FromBody] CreateAppointmentRequest createAppointmentRequest)
    {
        var result = await _userService.CreateNewAppoiment(createAppointmentRequest.UserId, createAppointmentRequest.DateAppointment);
        return Ok(result);
    }

    [HttpDelete]
    [Route("cancel-appointment/{appointmentId:guid}")]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId)
    {
        var result = await _userService.CancelAppoiment(appointmentId);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-appointments/{userId:guid}")]
    public async Task<IActionResult> GetAppointmentsByUser(Guid userId)
    {
        var result = await _userService.GetAppointmentsByUser(userId);
        return Ok(result);
    }
    
    [HttpPost]
    [Route("get-repostr/{userId:guid}")]
    public async Task<IActionResult> GetReportsByUser([FromRoute] Guid userId, [FromQuery] int page, [FromQuery] int pageZise)
    {
        var result = await _userService.GetReportsByUser(userId, page, pageZise);
        return Ok(new {Data = result.Value.Item1, Total = result.Value.Item2, result.Error, result.IsSuccess, result.Status});
    }
    
    
    [HttpGet]
    [Route("report/{reportId::guid}")]
    public async Task<IActionResult> DownloadReport(Guid reportId)
    {
        var result = await _pdfService.DownloadReport(reportId);
        if (!result.IsSuccess)
            return StatusCode(result.Status, result.Error);
        return File( result.Value.File, "application/pdf", result.Value.FileName);
    }
}