namespace TrainAlert.Api.Models;

public class TrainState
{
    public string TrainNumber { get; set; } = string.Empty;

    public int DelayMinutes { get; set; }

    public string? Platform { get; set; }

    public bool Running { get; set; }

    public bool NotDeparted { get; set; }

    public DateTime ObservedAt { get; set; }
    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;
    public bool Cancelled { get; set; }

    public string? DisruptionReason { get; set; }
}