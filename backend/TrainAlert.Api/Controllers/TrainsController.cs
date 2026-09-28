using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Services;

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
}