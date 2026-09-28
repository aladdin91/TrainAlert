using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public interface INotificationService
{
  Task NotifyAsync(TrainChange change);
}