using System.Security.Claims;
using AgtcSrvManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgtcSrvManagement.API.Controllers;

[ApiController]
[Route("/v1/api/sensors")]
public class SensorController : ControllerBase
{
    private readonly ILogger<SensorController> _logger;
    private readonly ISensorService _service;

    public SensorController(ILogger<SensorController> logger, ISensorService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> GetSensorsAsync()
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        var sensors = await _service.GetSensorsAsync(farmerId);
        return Ok(sensors);
    }

    [HttpDelete("{sensorId}")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> DeleteSensorsAsync(Guid sensorId)
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        await _service.DeleteSensorAsync(farmerId, sensorId);
        return NoContent();
    }
}
