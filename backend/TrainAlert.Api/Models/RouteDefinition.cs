namespace TrainAlert.Api.Models;

public class RouteDefinition
{
  public string OriginStationId { get; set; } = string.Empty;

  public string OriginStationName { get; set; } = string.Empty;

  public string DestinationStationId { get; set; } = string.Empty;

  public string DestinationStationName { get; set; } = string.Empty;

  public List<string> Stations { get; set; } = new();
}