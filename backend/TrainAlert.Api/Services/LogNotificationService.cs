using TrainAlert.Api.Data;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class LogNotificationService : INotificationService
{
  private readonly ApplicationDbContext _dbContext;
  private readonly ILogger<LogNotificationService> _logger;

  public LogNotificationService(
      ApplicationDbContext dbContext,
      ILogger<LogNotificationService> logger)
  {
    _dbContext = dbContext;
    _logger = logger;
  }

  public async Task NotifyAsync(
      Guid userId,
      Guid alertId,
      TrainChange change)
  {
    var notification = new Notification
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      AlertId = alertId,

      TrainNumber = change.TrainNumber,
      Origin = change.Origin,
      Destination = change.Destination,

      PreviousDelayMinutes = change.PreviousDelayMinutes,
      CurrentDelayMinutes = change.CurrentDelayMinutes,

      PreviousPlatform = change.PreviousPlatform,
      CurrentPlatform = change.CurrentPlatform,

      PreviousRunning = change.PreviousRunning,
      CurrentRunning = change.CurrentRunning,
      PreviousCancelled = change.PreviousCancelled,
      CurrentCancelled = change.CurrentCancelled,

      PreviousDisruptionReason =
    change.PreviousDisruptionReason,

      CurrentDisruptionReason =
    change.CurrentDisruptionReason,

      PreviousNotDeparted = change.PreviousNotDeparted,
      CurrentNotDeparted = change.CurrentNotDeparted,

      CreatedAt = DateTime.UtcNow,
      IsRead = false
    };

    _dbContext.Notifications.Add(notification);

    await _dbContext.SaveChangesAsync();

    _logger.LogInformation(
        "[NOTIFICATION] User {UserId}, Alert {AlertId}, Train {TrainNumber}: {Origin} -> {Destination}, delay {PreviousDelay} -> {CurrentDelay}, platform {PreviousPlatform} -> {CurrentPlatform}",
        userId,
        alertId,
        change.TrainNumber,
        change.Origin,
        change.Destination,
        change.PreviousDelayMinutes,
        change.CurrentDelayMinutes,
        change.PreviousPlatform,
        change.CurrentPlatform);
  }
}