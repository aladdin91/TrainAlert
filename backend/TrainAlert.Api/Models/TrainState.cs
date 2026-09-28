namespace TrainAlert.Api.Models;

public class TrainState
{
    public string TrainNumber { get; set; } = string.Empty;

    public int DelayMinutes { get; set; }

    public string? Platform { get; set; }

    public bool Running { get; set; }

    public bool NotDeparted { get; set; }

    public DateTime ObservedAt { get; set; }
}