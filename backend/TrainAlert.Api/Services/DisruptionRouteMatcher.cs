using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class DisruptionRouteMatcher
{
  public bool MatchesRoute(
      Disruption disruption,
      RouteDefinition route)
  {
    if (!disruption.IsActive)
    {
      return false;
    }

    if (disruption.Scope == "Train")
    {
      return false;
    }

    var text =
        $"{disruption.Title} {disruption.Description}"
            .ToLowerInvariant();

    var routeStations =
        route.Stations
            .Select(station =>
                station.ToLowerInvariant());

    return routeStations.Any(
        station => text.Contains(station));
  }
}