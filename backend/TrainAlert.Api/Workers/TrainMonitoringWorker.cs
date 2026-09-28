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

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

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

                var trains = await provider.GetTrainsAsync();

                _logger.LogInformation(
                  "Monitoring found {Count} trains",
                  trains.Count);

                foreach (var train in trains)
                {
                    var currentState = mapper.Map(train);

                    var previousState =
                        stateStore.Get(train.TrainNumber);

                    var change =
                        changeDetector.Detect(
                            previousState,
                            currentState);

                    if (change is not null)
                    {
                        _logger.LogInformation(
                            "Train {TrainNumber} changed: delay {PreviousDelay} -> {CurrentDelay}, platform {PreviousPlatform} -> {CurrentPlatform}",
                            change.TrainNumber,
                            change.PreviousDelayMinutes,
                            change.CurrentDelayMinutes,
                            change.PreviousPlatform,
                            change.CurrentPlatform);
                    }

                    stateStore.Set(currentState);
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