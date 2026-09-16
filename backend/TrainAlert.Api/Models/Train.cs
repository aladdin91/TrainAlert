namespace TrainAlert.Api.Models;

public class Train
{
    public string TrainNumber { get; set; } = string.Empty;

    public string Origin { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime ScheduledDeparture { get; set; }

    public DateTime? ActualDeparture { get; set; }

    public int DelayMinutes { get; set; }

    public string? Platform { get; set; }
}