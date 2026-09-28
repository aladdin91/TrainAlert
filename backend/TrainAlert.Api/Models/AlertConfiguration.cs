namespace TrainAlert.Api.Models;

public class AlertConfiguration
{
    public Guid Id { get; set; }

    public string OriginStationId { get; set; } = string.Empty;

    public string OriginStationName { get; set; } = string.Empty;

    public string DestinationStationId { get; set; } = string.Empty;

    public string DestinationStationName { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsEnabled { get; set; } = true;
}