using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class FakeTrainDataProvider : ITrainDataProvider
{
    public Task<List<Train>> GetTrainsAsync()
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
}