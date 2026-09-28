namespace TrainAlert.Api.Models;

public class TrainChange
{
    public string TrainNumber { get; set; } = string.Empty;

    public int? PreviousDelayMinutes { get; set; }

    public int? CurrentDelayMinutes { get; set; }

    public string? PreviousPlatform { get; set; }

    public string? CurrentPlatform { get; set; }

    public bool? PreviousRunning { get; set; }

    public bool? CurrentRunning { get; set; }

    public bool? PreviousNotDeparted { get; set; }

    public bool? CurrentNotDeparted { get; set; }
    public string Origin { get; set; } = string.Empty;

public string Destination { get; set; } = string.Empty;
}