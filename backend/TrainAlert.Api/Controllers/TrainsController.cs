using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Services;
using System.Text.RegularExpressions;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainsController : ControllerBase
{
    private readonly TrainService _trainService;

    public TrainsController(TrainService trainService)
    {
        _trainService = trainService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrains(
        [FromQuery] string stationId)
    {
        var trains =
            await _trainService.GetTrainsAsync(stationId);

        return Ok(trains);
    }

    [HttpGet("{trainNumber}")]
    public async Task<IActionResult> GetTrainDetails(
        string trainNumber,
        [FromQuery] string? originStationId,
        [FromQuery] long? departureDateEpochMilliseconds)
    {
        if (!IsValidJourneyRequest(
                trainNumber,
                originStationId,
                departureDateEpochMilliseconds))
        {
            return BadRequest(new
            {
                message = "A valid train number, originStationId, and departureDateEpochMilliseconds are required."
            });
        }

        var details = await _trainService.GetTrainDetailsAsync(
            originStationId!,
            trainNumber,
            departureDateEpochMilliseconds!.Value);

        return Ok(details);
    }

    [HttpGet("{trainNumber}/stops")]
    public async Task<IActionResult> GetTrainStops(
        string trainNumber,
        [FromQuery] string? originStationId,
        [FromQuery] long? departureDateEpochMilliseconds)
    {
        if (!IsValidJourneyRequest(
                trainNumber,
                originStationId,
                departureDateEpochMilliseconds))
        {
            return BadRequest(new
            {
                message = "A valid train number, originStationId, and departureDateEpochMilliseconds are required."
            });
        }

        var stops = await _trainService.GetTrainStopsAsync(
            originStationId!,
            trainNumber,
            departureDateEpochMilliseconds!.Value);

        return Ok(stops);
    }

    private static bool IsValidJourneyRequest(
        string trainNumber,
        string? originStationId,
        long? departureDateEpochMilliseconds)
    {
        return Regex.IsMatch(trainNumber, "^\\d+$") &&
               originStationId is not null &&
               Regex.IsMatch(originStationId, "^S\\d{5}$") &&
               departureDateEpochMilliseconds is > 0;
    }
}
