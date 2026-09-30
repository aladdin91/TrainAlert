using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class FakeTrainDataProvider : ITrainDataProvider
{
public Task<List<Train>> GetTrainsAsync(string stationId)
    {
        var trains = new List<Train>
        {
            new Train
            {
                TrainNumber = "12345",
                Origin = "Lecco",
                Destination = "Milano Porta Garibaldi",
                ScheduledDeparture = DateTime.Now.AddMinutes(10),
                ActualDeparture = null,
                DelayMinutes = 0,
                Platform = "2"
            },
            new Train
            {
                TrainNumber = "67890",
                Origin = "Lecco",
                Destination = "Milano Porta Garibaldi",
                ScheduledDeparture = DateTime.Now.AddMinutes(30),
                ActualDeparture = null,
                DelayMinutes = 7,
                Platform = "4"
            }
        };

        return Task.FromResult(trains);
    }

    public Task<List<Train>> GetArrivalsAsync(string stationId)
    {
        return Task.FromResult(new List<Train>());
    }

    public Task<TrainDetails> GetTrainDetailsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        return Task.FromResult(new TrainDetails());
    }

    public Task<List<TrainStop>> GetTrainStopsAsync(
        string originStationId,
        string trainNumber,
        long departureDateEpochMilliseconds)
    {
        return Task.FromResult(new List<TrainStop>());
    }
}
