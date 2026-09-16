using Microsoft.AspNetCore.Mvc;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            message = "TrainAlert API is running"
        });
    }
}