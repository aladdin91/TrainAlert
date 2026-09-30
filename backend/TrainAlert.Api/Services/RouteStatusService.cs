using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;
namespace TrainAlert.Api.Services;
public class RouteStatusService
{
  private readonly ITrainDataProvider _trains;
  private readonly IInfomobilityProvider _disruptions;
  private readonly DisruptionRouteMatcher _matcher;
  public RouteStatusService(ITrainDataProvider trains, IInfomobilityProvider disruptions, DisruptionRouteMatcher matcher) { _trains = trains; _disruptions = disruptions; _matcher = matcher; }
  public async Task<RouteStatus> GetStatusAsync(string originStationId, string destinationStationId)
  {
    var routeTrains = new List<Train>(); var routeStops = new List<TrainStop>();
    foreach (var train in await _trains.GetTrainsAsync(originStationId))
    {
      if (!train.DepartureDateEpochMilliseconds.HasValue) continue;
      var stops = await _trains.GetTrainStopsAsync(train.OriginStationId, train.TrainNumber, train.DepartureDateEpochMilliseconds.Value);
      if (stops.Any(s => string.Equals(s.StationId, destinationStationId, StringComparison.OrdinalIgnoreCase))) { routeTrains.Add(train); routeStops.AddRange(stops); }
    }
    var disruption = (await _disruptions.GetDisruptionsAsync()).FirstOrDefault(d => _matcher.MatchesRoute(d, routeStops));
    var cancelled = routeTrains.Count(t => t.Cancelled); var delayed = routeTrains.Count(t => t.DelayMinutes > 0);
    return new RouteStatus { Origin = originStationId, Destination = destinationStationId, Status = cancelled > 0 || disruption is not null ? "Disrupted" : delayed > 0 ? "Delayed" : "Stable", AverageDelayMinutes = routeTrains.Count == 0 ? 0 : Math.Round(routeTrains.Average(t => t.DelayMinutes), 1), TrainsMonitored = routeTrains.Count, DelayedTrains = delayed, CancelledTrains = cancelled, DisruptionReason = disruption?.Description, UpdatedAt = DateTime.UtcNow };
  }
}
