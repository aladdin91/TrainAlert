using System.Globalization;
using System.Text.Json;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class ViaggiaTrenoProvider : ITrainDataProvider, IStationSearchProvider
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

        var trains =
         JsonSerializer.Deserialize<List<ViaggiaTrenoTrainDto>>(json)
         ?? new List<ViaggiaTrenoTrainDto>();

        return trains
            .Select(dto => MapBoardTrain(dto, stationId, false))
            .ToList();
    }

    public async Task<List<Train>> GetArrivalsAsync(string stationId)
    {
        var client = _httpClientFactory.CreateClient();

        var dateTime = DateTime.Now.ToString(
            "ddd MMM d yyyy HH:mm:ss",
            CultureInfo.InvariantCulture);

        var url =
            $"http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/arrivi/{stationId}/{dateTime}";

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var trains =
            JsonSerializer.Deserialize<List<ViaggiaTrenoTrainDto>>(json)
            ?? new List<ViaggiaTrenoTrainDto>();

        return trains
            .Select(dto => MapBoardTrain(dto, stationId, true))
            .ToList();
    }

    public async Task<TrainDetails> GetTrainDetailsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        var details = await GetTrainDetailsDtoAsync(
            originStationId,
            trainNumber,
            departureDateEpochMilliseconds);

        return new TrainDetails
        {
            TrainNumber = details.TrainNumber.ToString(),
            Origin = details.Origin ?? string.Empty,
            OriginStationId = details.OriginStationId ?? string.Empty,
            Destination = details.Destination ?? string.Empty,
            DestinationStationId = details.DestinationStationId ?? string.Empty,
            ScheduledDeparture = ToDateTime(details.ScheduledDeparture),
            ScheduledArrival = ToDateTime(details.ScheduledArrival),
            DelayMinutes = details.DelayMinutes,
            Running = details.Running,
            NotDeparted = details.NotDeparted,
            Cancelled = details.TrainType is "ST" or "SF" or "SI",
            DisruptionReason = details.DisruptionReason
        };
    }

    public async Task<List<TrainStop>> GetTrainStopsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        var details = await GetTrainDetailsDtoAsync(
            originStationId,
            trainNumber,
            departureDateEpochMilliseconds);

        return details.Stops.Select(stop => new TrainStop
        {
            StationId = stop.StationId ?? string.Empty,
            StationName = stop.StationName ?? string.Empty,
            ScheduledArrival = ToDateTime(stop.ScheduledArrival),
            ScheduledDeparture = ToDateTime(stop.ScheduledDeparture),
            ActualArrival = ToDateTime(stop.ActualArrival),
            ActualDeparture = ToDateTime(stop.ActualDeparture),
            ArrivalPlatform = stop.ArrivalPlatform,
            DeparturePlatform = stop.DeparturePlatform,
            Sequence = stop.Sequence
        }).ToList();
    }

    public async Task<List<Station>> SearchStationsAsync(string query)
    {
        var client = _httpClientFactory.CreateClient();

        var url =
            "http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/cercaStazione/" +
            Uri.EscapeDataString(query);

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var stations =
            JsonSerializer.Deserialize<List<ViaggiaTrenoStationDto>>(json)
            ?? new List<ViaggiaTrenoStationDto>();

        return stations.Select(dto => new Station
        {
            StationId = dto.Id ?? string.Empty,
            LongName = dto.LongName ?? string.Empty,
            ShortName = dto.ShortName,
            Label = dto.Label
        }).ToList();
    }

    private async Task<ViaggiaTrenoTrainDetailsDto> GetTrainDetailsDtoAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        var client = _httpClientFactory.CreateClient();

        var url =
            "http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/andamentoTreno/" +
            $"{Uri.EscapeDataString(originStationId)}/" +
            $"{Uri.EscapeDataString(trainNumber)}/" +
            departureDateEpochMilliseconds;

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ViaggiaTrenoTrainDetailsDto>(json)
            ?? throw new InvalidOperationException(
                "ViaggiaTreno returned an empty train details response.");
    }

    private static Train MapBoardTrain(
        ViaggiaTrenoTrainDto dto,
        string stationId,
        bool isArrival)
    {
        return new Train
        {
            TrainNumber = dto.TrainNumber.ToString(),
            Origin = dto.Origin ?? string.Empty,
            OriginStationId = dto.OriginStationId ?? stationId,
            Destination = dto.Destination ?? string.Empty,
            ScheduledDeparture = isArrival
                ? DateTime.MinValue
                : ToDateTime(dto.DepartureTime) ?? DateTime.MinValue,
            ScheduledArrival = isArrival
                ? ToDateTime(dto.ArrivalTime)
                : null,
            ActualDeparture = null,
            ActualArrival = null,
            DepartureDateEpochMilliseconds =
                dto.DepartureDateEpochMilliseconds,
            DelayMinutes = dto.DelayMinutes,
            Platform = isArrival ? dto.ArrivalPlatform : dto.Platform,
            Running = dto.Running,
            NotDeparted = dto.NotDeparted,
            Cancelled = dto.Provision == 1 ||
                        dto.TrainType is "ST" or "SF" or "SI",
            DisruptionReason = null
        };
    }

    private static DateTime? ToDateTime(long? value)
    {
        return value.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(value.Value)
                .LocalDateTime
            : null;
    }
}
