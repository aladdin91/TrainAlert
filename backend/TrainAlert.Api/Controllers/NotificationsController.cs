using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrainAlert.Api.Services;

namespace TrainAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
  private readonly NotificationService _notificationService;

  public NotificationsController(
      NotificationService notificationService)
  {
    _notificationService = notificationService;
  }

  [HttpGet]
  public async Task<IActionResult> GetNotifications()
  {
    var userId = GetCurrentUserId();

    var notifications =
        await _notificationService.GetAllAsync(userId);

    return Ok(notifications);
  }

  [HttpPost("{id:guid}/read")]
  public async Task<IActionResult> MarkAsRead(Guid id)
  {
    var userId = GetCurrentUserId();

    var updated =
        await _notificationService.MarkAsReadAsync(
            id,
            userId);

    if (!updated)
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