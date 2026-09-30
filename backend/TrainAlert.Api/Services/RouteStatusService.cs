using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;

namespace TrainAlert.Api.Services;

public class RouteStatusService
{
  private readonly ITrainDataProvider _trainDataProvider;

  public RouteStatusService(
      ITrainDataProvider trainDataProvider)
  {
    _trainDataProvider = trainDataProvider;
  }

  public async Task<RouteStatus> GetStatusAsync(
      string originStationId,
      string destinationName)
  {
    var trains =
        await _trainDataProvider.GetTrainsAsync(
            originStationId);

    var routeTrains = trains
        .Where(train =>
            string.Equals(
                train.Destination,
                destinationName,
                StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (routeTrains.Count == 0)
    {
      return new RouteStatus
      {
        Origin = originStationId,
        Destination = destinationName,
        Status = "Unknown",
        AverageDelayMinutes = 0,
        TrainsMonitored = 0,
        DelayedTrains = 0,
        UpdatedAt = DateTime.UtcNow
      };
    }

    var averageDelay =
        routeTrains.Average(
            train => train.DelayMinutes);

    var delayedTrains =
        routeTrains.Count(
            train => train.DelayMinutes > 0);

    var cancelledTrains =
        routeTrains.Count(
            train => train.Cancelled);

    var status =
        cancelledTrains > 0
            ? "Disrupted"
            : delayedTrains > 0
                ? "Delayed"
                : "Stable";

    return new RouteStatus
    {
      Origin = originStationId,
      Destination = destinationName,

      Status = status,

      AverageDelayMinutes =
         Math.Round(averageDelay, 1),

      TrainsMonitored =
         routeTrains.Count,

      DelayedTrains =
         delayedTrains,

      CancelledTrains =
         cancelledTrains,

      DisruptionReason =
         cancelledTrains > 0
             ? "One or more trains are cancelled."
             : null,

      UpdatedAt = DateTime.UtcNow
    };
  }
}