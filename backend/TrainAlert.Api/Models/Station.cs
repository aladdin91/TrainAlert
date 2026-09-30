namespace TrainAlert.Api.Models;

public class Station
{
    public string StationId { get; set; } = string.Empty;

    public string LongName { get; set; } = string.Empty;

    public string? ShortName { get; set; }

    public string? Label { get; set; }

    public string DisplayName => Label ?? ShortName ?? LongName;
}
