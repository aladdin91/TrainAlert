
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
      [FromQuery] string destinationStationId)
  {
    if (!IsValidStationId(originStationId) ||
        !IsValidStationId(destinationStationId))
    {
      return BadRequest(new
      {
        message =
              "Valid originStationId and destinationStationId are required."
      });
    }

    var status =
        await _routeStatusService.GetStatusAsync(
            originStationId,
            destinationStationId);

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

  private static bool IsValidStationId(string? stationId)
  {
    return stationId is not null &&
           Regex.IsMatch(stationId, "^S\\d{5}$");
  }
}
