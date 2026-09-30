using System.Text.Json.Serialization;

namespace TrainAlert.Api.Models;

public class ViaggiaTrenoTrainDto
{
    [JsonPropertyName("numeroTreno")]
    public int TrainNumber { get; set; }

    [JsonPropertyName("destinazione")]
    public string? Destination { get; set; }

    [JsonPropertyName("origine")]
    public string? Origin { get; set; }

    [JsonPropertyName("codOrigine")]
    public string? OriginStationId { get; set; }

    [JsonPropertyName("dataPartenzaTreno")]
    public long? DepartureDateEpochMilliseconds { get; set; }

    [JsonPropertyName("orarioPartenza")]
    public long? DepartureTime { get; set; }

    [JsonPropertyName("orarioArrivo")]
    public long? ArrivalTime { get; set; }

    [JsonPropertyName("ritardo")]
    public int DelayMinutes { get; set; }

    [JsonPropertyName("binarioProgrammatoPartenzaDescrizione")]
    public string? Platform { get; set; }

    [JsonPropertyName("binarioProgrammatoArrivoDescrizione")]
    public string? ArrivalPlatform { get; set; }

    [JsonPropertyName("circolante")]
    public bool Running { get; set; }

    [JsonPropertyName("nonPartito")]
    public bool NotDeparted { get; set; }

    [JsonPropertyName("provvedimento")]
    public int Provision { get; set; }

    [JsonPropertyName("tipoTreno")]
    public string? TrainType { get; set; }
}
