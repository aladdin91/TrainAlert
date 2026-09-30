using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public interface ITrainDataProvider
{
Task<List<Train>> GetTrainsAsync(string stationId);
Task<List<Train>> GetArrivalsAsync(string stationId);
Task<TrainDetails> GetTrainDetailsAsync(
    string originStationId,
    string trainNumber,
    long departureDateEpochMilliseconds);
Task<List<TrainStop>> GetTrainStopsAsync(
    string originStationId,
    string trainNumber,
    long departureDateEpochMilliseconds);
}
