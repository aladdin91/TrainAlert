using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Services;
using System.Text.RegularExpressions;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StationsController : ControllerBase
{
    private readonly StationSearchService _stationSearchService;
    private readonly TrainService _trainService;

    public StationsController(
        StationSearchService stationSearchService,
        TrainService trainService)
    {
        _stationSearchService = stationSearchService;
        _trainService = trainService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new
            {
                message = "query is required."
            });
        }

        var stations = await _stationSearchService.SearchAsync(query.Trim());

        return Ok(stations);
    }

    [HttpGet("{stationId}/departures")]
    public async Task<IActionResult> GetDepartures(
        string stationId,
        [FromQuery] string? destinationStationId)
    {
        if (!IsValidStationId(stationId) ||
            (destinationStationId is not null &&
             !IsValidStationId(destinationStationId)))
        {
            return BadRequest(new
            {
                message = "A valid station ID is required."
            });
        }

        var departures = await _trainService.GetDeparturesAsync(
            stationId,
            destinationStationId);

        return Ok(departures);
    }

    [HttpGet("{stationId}/arrivals")]
    public async Task<IActionResult> GetArrivals(string stationId)
    {
        if (!IsValidStationId(stationId))
        {
            return BadRequest(new
            {
                message = "A valid station ID is required."
            });
        }

        var arrivals = await _trainService.GetArrivalsAsync(stationId);

        return Ok(arrivals);
    }

    private static bool IsValidStationId(string? stationId)
    {
        return stationId is not null &&
               Regex.IsMatch(stationId, "^S\\d{5}$");
    }
}
