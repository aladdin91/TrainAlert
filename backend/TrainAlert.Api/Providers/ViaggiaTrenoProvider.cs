using System.Globalization;
using System.Text.Json;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class ViaggiaTrenoProvider : ITrainDataProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ViaggiaTrenoProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

public async Task<List<Train>> GetTrainsAsync(string stationId)
    {
        var client = _httpClientFactory.CreateClient();

        var dateTime = DateTime.Now.ToString(
            "ddd MMM d yyyy HH:mm:ss",
            CultureInfo.InvariantCulture
        );

        var url =
        $"http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/partenze/{stationId}/{dateTime}";

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var trains = JsonSerializer.Deserialize<List<ViaggiaTrenoTrainDto>>(json)
             ?? new List<ViaggiaTrenoTrainDto>();

return trains.Select(dto => new Train
{
    TrainNumber = dto.TrainNumber.ToString(),

    Origin = stationId,

    DestinationStationId = dto.DestinationStationId,

    Destination = dto.Destination ?? string.Empty,

    ScheduledDeparture = dto.DepartureTime.HasValue
        ? DateTimeOffset
            .FromUnixTimeMilliseconds(dto.DepartureTime.Value)
            .LocalDateTime
        : DateTime.MinValue,

    ActualDeparture = null,
    DelayMinutes = dto.DelayMinutes,
    Platform = dto.Platform,
    Running = dto.Running,
    NotDeparted = dto.NotDeparted
}).ToList();
    }
}