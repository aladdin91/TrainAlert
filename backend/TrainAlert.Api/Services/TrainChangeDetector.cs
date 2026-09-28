using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class TrainChangeDetector
{
    public TrainChange? Detect(TrainState? previous, TrainState current)
    {
        if (previous is null)
        {
            return null;
        }

        var delayChanged =
            previous.DelayMinutes != current.DelayMinutes;

        var platformChanged =
            previous.Platform != current.Platform;

        var runningChanged =
            previous.Running != current.Running;

        var notDepartedChanged =
            previous.NotDeparted != current.NotDeparted;

        if (!delayChanged &&
            !platformChanged &&
            !runningChanged &&
            !notDepartedChanged)
        {
            return null;
        }

        return new TrainChange
        {
            TrainNumber = current.TrainNumber,

            PreviousDelayMinutes = previous.DelayMinutes,
            CurrentDelayMinutes = current.DelayMinutes,

            PreviousPlatform = previous.Platform,
            CurrentPlatform = current.Platform,

            PreviousRunning = previous.Running,
            CurrentRunning = current.Running,

            PreviousNotDeparted = previous.NotDeparted,
            CurrentNotDeparted = current.NotDeparted
        };
    }
}