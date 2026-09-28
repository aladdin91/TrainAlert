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
    public IActionResult GetAlerts()
    {
        return Ok(_alertService.GetAll());
    }

    [HttpPost]
    public IActionResult CreateAlert(AlertConfiguration alert)
    {
        var createdAlert = _alertService.Add(alert);

        return CreatedAtAction(
            nameof(GetAlert),
            new { id = createdAlert.Id },
            createdAlert);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetAlert(Guid id)
    {
        var alert = _alertService.GetById(id);

        if (alert is null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteAlert(Guid id)
    {
        var deleted = _alertService.Delete(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}