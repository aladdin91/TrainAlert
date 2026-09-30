using System.Text.Json.Serialization;

namespace TrainAlert.Api.Models;

public class ViaggiaTrenoStationDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("nomeLungo")]
    public string? LongName { get; set; }

    [JsonPropertyName("nomeBreve")]
    public string? ShortName { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }
}
