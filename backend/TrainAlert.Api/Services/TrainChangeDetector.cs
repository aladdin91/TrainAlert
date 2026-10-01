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

        var cancelledChanged =
            previous.Cancelled != current.Cancelled;

        var disruptionReasonChanged =
            previous.DisruptionReason != current.DisruptionReason;

        if (!delayChanged &&
       !platformChanged &&
       !runningChanged &&
       !notDepartedChanged &&
       !cancelledChanged &&
       !disruptionReasonChanged)
        {
            return null;
        }

        return new TrainChange
        {
            TrainNumber = current.TrainNumber,
            Origin = current.Origin,
            Destination = current.Destination,

            PreviousDelayMinutes = previous.DelayMinutes,
            CurrentDelayMinutes = current.DelayMinutes,

            PreviousPlatform = previous.Platform,
            CurrentPlatform = current.Platform,

            PreviousRunning = previous.Running,
            CurrentRunning = current.Running,

            PreviousNotDeparted = previous.NotDeparted,
            CurrentNotDeparted = current.NotDeparted,
            PreviousCancelled = previous.Cancelled,
            CurrentCancelled = current.Cancelled,

            PreviousDisruptionReason = previous.DisruptionReason,
            CurrentDisruptionReason = current.DisruptionReason,
        };
    }
}