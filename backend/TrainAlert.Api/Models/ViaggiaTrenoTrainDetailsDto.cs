using System.Text.Json.Serialization;

namespace TrainAlert.Api.Models;

public class ViaggiaTrenoTrainDetailsDto
{
    [JsonPropertyName("numeroTreno")]
    public int TrainNumber { get; set; }

    [JsonPropertyName("origine")]
    public string? Origin { get; set; }

    [JsonPropertyName("idOrigine")]
    public string? OriginStationId { get; set; }

    [JsonPropertyName("destinazione")]
    public string? Destination { get; set; }

    [JsonPropertyName("idDestinazione")]
    public string? DestinationStationId { get; set; }

    [JsonPropertyName("orarioPartenza")]
    public long? ScheduledDeparture { get; set; }

    [JsonPropertyName("orarioArrivo")]
    public long? ScheduledArrival { get; set; }

    [JsonPropertyName("ritardo")]
    public int DelayMinutes { get; set; }

    [JsonPropertyName("circolante")]
    public bool Running { get; set; }

    [JsonPropertyName("nonPartito")]
    public bool NotDeparted { get; set; }

    [JsonPropertyName("tipoTreno")]
    public string? TrainType { get; set; }

    [JsonPropertyName("motivoRitardoPrevalente")]
    public string? DisruptionReason { get; set; }

    [JsonPropertyName("fermate")]
    public List<ViaggiaTrenoTrainStopDto> Stops { get; set; } = new();
}

public class ViaggiaTrenoTrainStopDto
{
    [JsonPropertyName("id")]
    public string? StationId { get; set; }

    [JsonPropertyName("stazione")]
    public string? StationName { get; set; }

    [JsonPropertyName("arrivo_teorico")]
    public long? ScheduledArrival { get; set; }

    [JsonPropertyName("partenza_teorica")]
    public long? ScheduledDeparture { get; set; }

    [JsonPropertyName("arrivoReale")]
    public long? ActualArrival { get; set; }

    [JsonPropertyName("partenzaReale")]
    public long? ActualDeparture { get; set; }

    [JsonPropertyName("binarioEffettivoArrivoDescrizione")]
    public string? ArrivalPlatform { get; set; }

    [JsonPropertyName("binarioEffettivoPartenzaDescrizione")]
    public string? DeparturePlatform { get; set; }

    [JsonPropertyName("progressivo")]
    public int Sequence { get; set; }
}
