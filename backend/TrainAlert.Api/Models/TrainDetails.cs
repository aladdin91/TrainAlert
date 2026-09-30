namespace TrainAlert.Api.Models;

public class TrainDetails
{
    public string TrainNumber { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string OriginStationId { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public string DestinationStationId { get; set; } = string.Empty;

    public DateTime? ScheduledDeparture { get; set; }

    public DateTime? ScheduledArrival { get; set; }

    public int DelayMinutes { get; set; }

    public bool Running { get; set; }

    public bool NotDeparted { get; set; }

    public bool Cancelled { get; set; }

    public string? DisruptionReason { get; set; }
}
