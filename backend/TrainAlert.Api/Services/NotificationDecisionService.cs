using Microsoft.EntityFrameworkCore;

using TrainAlert.Api.Data;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class NotificationDecisionService
{
  private readonly ApplicationDbContext _dbContext;

  public NotificationDecisionService(
      ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<bool> ShouldNotifyAsync(
      Guid alertId,
      TrainChange change)
  {
    if (HasImmediateChange(change))
    {
      return true;
    }

    if (!HasDelayChange(change))
    {
      return false;
    }

    var previousDelay =
        change.PreviousDelayMinutes ?? 0;

    var currentDelay =
        change.CurrentDelayMinutes ?? 0;

    if (currentDelay <= previousDelay)
    {
      return false;
    }

    var previousThreshold =
        GetDelayThreshold(previousDelay);

    var currentThreshold =
        GetDelayThreshold(currentDelay);

    if (currentThreshold <= previousThreshold)
    {
      return false;
    }

    var lastNotification =
        await _dbContext.Notifications
            .AsNoTracking()
            .Where(notification =>
                notification.AlertId == alertId &&
                notification.TrainNumber == change.TrainNumber &&
                notification.CurrentDelayMinutes != null)
            .OrderByDescending(notification =>
                notification.CreatedAt)
            .FirstOrDefaultAsync();

    if (lastNotification?.CurrentDelayMinutes is int lastDelay)
    {
      var lastThreshold =
          GetDelayThreshold(lastDelay);

      if (currentThreshold <= lastThreshold)
      {
        return false;
      }
    }

    return true;
  }

  private static bool HasImmediateChange(
     TrainChange change)
  {
    return
        change.PreviousPlatform != change.CurrentPlatform ||
        change.PreviousRunning != change.CurrentRunning ||
        change.PreviousNotDeparted != change.CurrentNotDeparted ||
        change.PreviousCancelled != change.CurrentCancelled;
  }

  private static bool HasDelayChange(
      TrainChange change)
  {
    return
        change.PreviousDelayMinutes !=
        change.CurrentDelayMinutes;
  }

  private static int GetDelayThreshold(
      int delayMinutes)
  {
    if (delayMinutes < 5)
    {
      return 0;
    }

    return delayMinutes / 5;
  }
}