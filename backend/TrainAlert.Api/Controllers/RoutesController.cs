
using Microsoft.AspNetCore.Mvc;

using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
  private readonly RouteStatusService _routeStatusService;
  private readonly IInfomobilityProvider _infomobilityProvider;

  public RoutesController(
      RouteStatusService routeStatusService,
      IInfomobilityProvider infomobilityProvider)
  {
    _routeStatusService = routeStatusService;
    _infomobilityProvider = infomobilityProvider;
  }

  [HttpGet("status")]
  public async Task<IActionResult> GetStatus(
      [FromQuery] string originStationId,
      [FromQuery] string destinationName)
  {
    if (string.IsNullOrWhiteSpace(originStationId) ||
        string.IsNullOrWhiteSpace(destinationName))
    {
      return BadRequest(new
      {
        message =
              "originStationId and destinationName are required."
      });
    }

    var status =
        await _routeStatusService.GetStatusAsync(
            originStationId,
            destinationName);

    return Ok(status);
  }

  [HttpGet("disruptions")]
  public async Task<IActionResult> GetDisruptions()
  {
    var disruptions =
        await _infomobilityProvider
            .GetDisruptionsAsync();

    return Ok(disruptions);
  }
}
