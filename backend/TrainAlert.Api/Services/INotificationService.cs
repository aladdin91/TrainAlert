using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public interface INotificationService
{
  Task NotifyAsync(
      Guid userId,
      Guid alertId,
      TrainChange change);
}