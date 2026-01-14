using System.Security.Claims;
using AgtcSrvManagement.Application.Dtos;
using AgtcSrvManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgtcSrvManagement.API.Controllers;

[ApiController]
[Route("/v1/api/properties")]
public class PropertyController : ControllerBase
{
    private readonly ILogger<PropertyController> _logger;
    private readonly IPropertyService _service;

    public PropertyController(ILogger<PropertyController> logger, IPropertyService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> GetPropertiesAsync()
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        var properties = await _service.GetPropertiesAsync(farmerId);
        return Ok(properties);
    }

    [HttpGet("{propertyId}")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> GetPropertyAsync(Guid propertyId)
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        var property = await _service.GetPropertyAsync(farmerId, propertyId);
        return Ok(property);
    }

    [HttpPost]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> CreatePropertyAsync([FromBody] CreatePropertyRequest request)
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        await _service.CreatePropertyAsync(request, farmerId);
        return Created();
    }

    [HttpPost("{propertyId}")]
    [Authorize(Roles = "Farmer")]
    public async Task<IActionResult> AddFieldToPropertyAsync(Guid propertyId, [FromBody] CreateFieldRequest request)
    {
        var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
        await _service.AddFieldToPropertyAsync(request, propertyId, farmerId);
        return Created();
    }
}
