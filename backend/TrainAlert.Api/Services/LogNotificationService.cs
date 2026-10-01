using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class LogNotificationService : INotificationService
{
  private readonly ILogger<LogNotificationService> _logger;

  public LogNotificationService(
      ILogger<LogNotificationService> logger)
  {
    _logger = logger;
  }

  public Task NotifyAsync(
     Guid userId,
     Guid alertId,
     TrainChange change)
  {
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

    return Task.CompletedTask;
  }
}