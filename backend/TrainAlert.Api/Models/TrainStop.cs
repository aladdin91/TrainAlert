namespace TrainAlert.Api.Models;

public class TrainStop
{
    public string StationId { get; set; } = string.Empty;

    public string StationName { get; set; } = string.Empty;

    public DateTime? ScheduledArrival { get; set; }

    public DateTime? ScheduledDeparture { get; set; }

    public DateTime? ActualArrival { get; set; }

    public DateTime? ActualDeparture { get; set; }

    public string? ArrivalPlatform { get; set; }

    public string? DeparturePlatform { get; set; }

    public int Sequence { get; set; }
}
