namespace TrainAlert.Api.Models;

public class RouteStatus
{
  public string Origin { get; set; } = string.Empty;

  public string Destination { get; set; } = string.Empty;

  public string Status { get; set; } = string.Empty;

  public double AverageDelayMinutes { get; set; }

  public int TrainsMonitored { get; set; }

  public int DelayedTrains { get; set; }

  public int CancelledTrains { get; set; }

  public string? DisruptionReason { get; set; }

  public DateTime UpdatedAt { get; set; }
}