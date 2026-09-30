using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class DisruptionRouteMatcher
{
  public bool MatchesRoute(
      Disruption disruption,
      IEnumerable<TrainStop> stops)
  {
    if (!disruption.IsActive)
    {
      return false;
    }

    if (disruption.Scope is "Train" or "Network")
    {
      return false;
    }

    var text =
        $"{disruption.Title} {disruption.Description}"
            .ToLowerInvariant();

    var routeStations = stops
        .Select(stop => stop.StationName)
        .Where(station => !string.IsNullOrWhiteSpace(station))
        .Select(station => station.ToLowerInvariant())
        .Distinct(StringComparer.OrdinalIgnoreCase);

    return routeStations.Any(
        station => text.Contains(station));
  }
}
