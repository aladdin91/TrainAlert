using System.Text.Json.Serialization;

namespace TrainAlert.Api.Models;

public class ViaggiaTrenoTrainDto
{
    [JsonPropertyName("numeroTreno")]
    public int TrainNumber { get; set; }

    [JsonPropertyName("destinazione")]
    public string? Destination { get; set; }

[JsonPropertyName("codDestinazione")]
public string? DestinationStationId { get; set; }

    [JsonPropertyName("orarioPartenza")]
    public long? DepartureTime { get; set; }

    [JsonPropertyName("ritardo")]
    public int DelayMinutes { get; set; }

    [JsonPropertyName("binarioProgrammatoPartenzaDescrizione")]
    public string? Platform { get; set; }

    [JsonPropertyName("circolante")]
    public bool Running { get; set; }

    [JsonPropertyName("nonPartito")]
    public bool NotDeparted { get; set; }
}