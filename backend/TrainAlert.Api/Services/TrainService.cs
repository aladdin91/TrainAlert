using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;

namespace TrainAlert.Api.Services;

public class TrainService
{
    private readonly ITrainDataProvider _trainDataProvider;
    private readonly TrainDestinationFilter _destinationFilter;

    public TrainService(
        ITrainDataProvider trainDataProvider,
        TrainDestinationFilter destinationFilter)
    {
        _trainDataProvider = trainDataProvider;
        _destinationFilter = destinationFilter;
    }

    public Task<List<Train>> GetTrainsAsync(string stationId)
    {
        return _trainDataProvider.GetTrainsAsync(stationId);
    }

    public async Task<List<Train>> GetDeparturesAsync(
        string stationId,
        string? destinationStationId)
    {
        var trains = await _trainDataProvider.GetTrainsAsync(stationId);

        if (string.IsNullOrWhiteSpace(destinationStationId))
        {
            return trains;
        }

        var matchingTrains = new List<Train>();

        foreach (var train in trains)
        {
            if (!train.DepartureDateEpochMilliseconds.HasValue)
            {
                continue;
            }

            var stops = await _trainDataProvider.GetTrainStopsAsync(
                train.OriginStationId,
                train.TrainNumber,
                train.DepartureDateEpochMilliseconds.Value);

            if (_destinationFilter.StopsAtDestination(
                    stops,
                    destinationStationId))
            {
                matchingTrains.Add(train);
            }
        }

        return matchingTrains;
    }

    public Task<List<Train>> GetArrivalsAsync(string stationId)
    {
        return _trainDataProvider.GetArrivalsAsync(stationId);
    }

    public Task<TrainDetails> GetTrainDetailsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        return _trainDataProvider.GetTrainDetailsAsync(
            originStationId,
            trainNumber,
            departureDateEpochMilliseconds);
    }

    public Task<List<TrainStop>> GetTrainStopsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        return _trainDataProvider.GetTrainStopsAsync(
            originStationId,
            trainNumber,
            departureDateEpochMilliseconds);
    }
}
