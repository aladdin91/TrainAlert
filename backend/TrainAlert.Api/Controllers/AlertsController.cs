using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly AlertService _alertService;

    public AlertsController(AlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var alerts = await _alertService.GetAllAsync();

        return Ok(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(
        AlertConfiguration alert)
    {
        var createdAlert =
            await _alertService.AddAsync(alert);

        return CreatedAtAction(
            nameof(GetAlert),
            new { id = createdAlert.Id },
            createdAlert);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAlert(Guid id)
    {
        var alert =
            await _alertService.GetByIdAsync(id);

        if (alert is null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAlert(Guid id)
    {
        var deleted =
            await _alertService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}