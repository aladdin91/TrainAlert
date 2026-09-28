using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class TrainStateMapper
{
    public TrainState Map(Train train)
    {
        return new TrainState
        {
            TrainNumber = train.TrainNumber,
            DelayMinutes = train.DelayMinutes,
            Platform = train.Platform,
            Running = train.Running,
            NotDeparted = train.NotDeparted,
            ObservedAt = DateTime.UtcNow
        };
    }
}