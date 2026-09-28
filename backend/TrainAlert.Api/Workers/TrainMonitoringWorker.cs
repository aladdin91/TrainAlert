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

                var alerts = alertService
                    .GetAll()
                    .Where(alert => alert.IsEnabled)
                    .ToList();

                var provider =
                    scope.ServiceProvider
                        .GetRequiredService<ITrainDataProvider>();

                var stateStore =
                    scope.ServiceProvider
                        .GetRequiredService<TrainStateStore>();

                var mapper =
                    scope.ServiceProvider
                        .GetRequiredService<TrainStateMapper>();

                var changeDetector =
                    scope.ServiceProvider
                        .GetRequiredService<TrainChangeDetector>();

                foreach (var alert in alerts)
                {
                    var trains =
                        await provider.GetTrainsAsync(
                            alert.OriginStationId);

var filteredTrains = trains
    .Where(train =>
        string.Equals(
            train.Destination,
            alert.DestinationStationName,
            StringComparison.OrdinalIgnoreCase))
    .Where(train =>
    {
        var departureTime =
            TimeOnly.FromDateTime(
                train.ScheduledDeparture);

        return departureTime >= alert.StartTime &&
               departureTime <= alert.EndTime;
    })
    .ToList();

                    _logger.LogInformation(
    "Alert {AlertId}: found {Count} matching trains: {Origin} -> {Destination}, between {StartTime} and {EndTime}",
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
                                train.TrainNumber);

                        var change =
                            changeDetector.Detect(
                                previousState,
                                currentState);

                        if (change is not null)
                        {
                            _logger.LogInformation(
                                "Train {TrainNumber} changed: {Origin} -> {Destination}, delay {PreviousDelay} -> {CurrentDelay}, platform {PreviousPlatform} -> {CurrentPlatform}",
                                change.TrainNumber,
                                change.Origin,
                                change.Destination,
                                change.PreviousDelayMinutes,
                                change.CurrentDelayMinutes,
                                change.PreviousPlatform,
                                change.CurrentPlatform);
                        }

                        stateStore.Set(currentState);
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