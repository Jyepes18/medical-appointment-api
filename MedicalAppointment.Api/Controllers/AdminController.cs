using MedicalAppointment.Api.Interfaces;
using MedicalAppointment.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost]
    [Route("create-user")]
    public async Task<IActionResult> CreateUser(User user)
    {
        var result = await _adminService.CreateUser(user);
        return Ok(result);
    }
    
    [HttpGet]
    [Route("get-user/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _adminService.GetUserById(id);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-all-users/{roleId:guid}")]
    public async Task<IActionResult> GetAllUsers(Guid roleId)
    {
        var result = await _adminService.GetAllUsers(roleId);
        return Ok(new {Data = result.Value.Item1, Total = result.Value.Item2, result.Status, result.IsSuccess, result.Error});
    }

    [HttpPut]
    [Route("update-user")]
    public async Task<IActionResult> UpdateUser(User user)
    {
        var result = await _adminService.UpdateUser(user);
        return Ok(result);
    }

    [HttpDelete]
    [Route("deactivate-user/{id:guid}")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        var result = await _adminService.DeactivateUser(id);
        return Ok(result);
    }
    
    [HttpPut]
    [Route("activate-user/{id:guid}")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        var result = await _adminService.ActivateUser(id);
        return Ok(result);
    }
}