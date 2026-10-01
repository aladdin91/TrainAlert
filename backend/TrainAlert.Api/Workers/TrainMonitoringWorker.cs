using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Workers;

public class TrainMonitoringWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TrainMonitoringWorker> _logger;

    public TrainMonitoringWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<TrainMonitoringWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public static bool ShouldProcessAlert(
        AlertConfiguration alert,
        TimeOnly currentTime)
    {
        if (!alert.IsEnabled)
        {
            return false;
        }

        if (alert.StartTime < alert.EndTime)
        {
            return currentTime >= alert.StartTime &&
                   currentTime < alert.EndTime;
        }

        if (alert.StartTime > alert.EndTime)
        {
            return currentTime >= alert.StartTime ||
                   currentTime < alert.EndTime;
        }

        return false;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var alertService =
                    scope.ServiceProvider
                        .GetRequiredService<AlertService>();

                var alerts =
                    await alertService.GetAllForMonitoringAsync();

                var provider =
                    scope.ServiceProvider
                        .GetRequiredService<ITrainDataProvider>();

                var destinationFilter =
                    scope.ServiceProvider
                        .GetRequiredService<TrainDestinationFilter>();

                var stateStore =
                    scope.ServiceProvider
                        .GetRequiredService<TrainStateStore>();

                var mapper =
                    scope.ServiceProvider
                        .GetRequiredService<TrainStateMapper>();

                var changeDetector =
                    scope.ServiceProvider
                        .GetRequiredService<TrainChangeDetector>();

                var notificationService =
                    scope.ServiceProvider
                        .GetRequiredService<INotificationService>();

                var currentTime =
                    TimeOnly.FromDateTime(DateTime.Now);

                foreach (var alert in alerts)
                {
                    if (!ShouldProcessAlert(alert, currentTime))
                    {
                        continue;
                    }

                    var trains =
                        await provider.GetTrainsAsync(
                            alert.OriginStationId);

                    var filteredTrains = new List<Train>();

                    foreach (var train in trains)
                    {
                        var departureTime =
                            TimeOnly.FromDateTime(
                                train.ScheduledDeparture);

                        if (departureTime < alert.StartTime ||
                            departureTime > alert.EndTime)
                        {
                            continue;
                        }

                        if (!train.DepartureDateEpochMilliseconds.HasValue)
                        {
                            continue;
                        }

                        var stops =
                            await provider.GetTrainStopsAsync(
                                train.OriginStationId,
                                train.TrainNumber,
                                train.DepartureDateEpochMilliseconds.Value);

                        if (destinationFilter.StopsAtDestination(
                                stops,
                                alert.DestinationStationId))
                        {
                            filteredTrains.Add(train);
                        }
                    }

                    _logger.LogInformation(
                        "[{Time}] Alert {AlertId}: found {Count} matching trains: {Origin} -> {Destination}, between {StartTime} and {EndTime}",
                        DateTime.Now.ToString("HH:mm:ss"),
                        alert.Id,
                        filteredTrains.Count,
                        alert.OriginStationName,
                        alert.DestinationStationName,
                        alert.StartTime,
                        alert.EndTime);

                    foreach (var train in filteredTrains)
                    {
                        var currentState =
                            mapper.Map(train);

                        var previousState =
                            stateStore.Get(
                                alert.Id,
                                train.TrainNumber);

                        var change =
                            changeDetector.Detect(
                                previousState,
                                currentState);

                        if (change is not null)
                        {
                            _logger.LogInformation(
                                "[{Time}] Train {TrainNumber} changed: {Origin} -> {Destination}, delay {PreviousDelay} -> {CurrentDelay}, platform {PreviousPlatform} -> {CurrentPlatform}",
                                DateTime.Now.ToString("HH:mm:ss"),
                                change.TrainNumber,
                                change.Origin,
                                change.Destination,
                                change.PreviousDelayMinutes,
                                change.CurrentDelayMinutes,
                                change.PreviousPlatform,
                                change.CurrentPlatform);

                            await notificationService.NotifyAsync(
                                alert.UserId,
                                alert.Id,
                                change);
                        }
                        else if (previousState is null)
                        {
                            _logger.LogInformation(
                                "Tracking train {TrainNumber}: {Origin} -> {Destination}, delay {Delay}, platform {Platform}",
                                currentState.TrainNumber,
                                currentState.Origin,
                                currentState.Destination,
                                currentState.DelayMinutes,
                                currentState.Platform);
                        }

                        stateStore.Set(
                            alert.Id,
                            currentState);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while monitoring trains");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
        }
    }
}