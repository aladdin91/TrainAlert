using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var userId = GetCurrentUserId();

        var alerts =
            await _alertService.GetAllAsync(userId);

        return Ok(alerts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert(
        AlertConfiguration alert)
    {
        var userId = GetCurrentUserId();

        alert.UserId = userId;

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
        var userId = GetCurrentUserId();

        var alert =
            await _alertService.GetByIdAsync(id, userId);

        if (alert is null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAlert(Guid id)
    {
        var userId = GetCurrentUserId();

        var deleted =
            await _alertService.DeleteAsync(id, userId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new InvalidOperationException(
                "User ID claim is missing.");
        }

        return parsedUserId;
    }
}