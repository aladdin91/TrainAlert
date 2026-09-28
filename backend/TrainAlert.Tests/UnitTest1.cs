using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class TrainChangeDetectorTests
{
    private readonly TrainChangeDetector _detector = new();

    private static TrainState CreateState(
        int delay = 0,
        string? platform = "1",
        bool running = true,
        bool notDeparted = true)
    {
        return new TrainState
        {
            TrainNumber = "24879",
            Origin = "CARNATE USMATE",
            Destination = "MILANO PORTA GARIBALDI",
            DelayMinutes = delay,
            Platform = platform,
            Running = running,
            NotDeparted = notDeparted,
            ObservedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public void FirstObservation_ShouldNotCreateChange()
    {
        var current = CreateState();

        var change = _detector.Detect(null, current);

        Assert.Null(change);
    }

    [Fact]
    public void DelayIncrease_ShouldCreateChange()
    {
        var previous = CreateState(delay: 0);
        var current = CreateState(delay: 1);

        var change = _detector.Detect(previous, current);

        Assert.NotNull(change);
        Assert.Equal(0, change.PreviousDelayMinutes);
        Assert.Equal(1, change.CurrentDelayMinutes);
    }

    [Fact]
    public void DelayDecrease_ShouldCreateChange()
    {
        var previous = CreateState(delay: 2);
        var current = CreateState(delay: 1);

        var change = _detector.Detect(previous, current);

        Assert.NotNull(change);
        Assert.Equal(2, change.PreviousDelayMinutes);
        Assert.Equal(1, change.CurrentDelayMinutes);
    }

    [Fact]
    public void SameDelay_ShouldNotCreateChange()
    {
        var previous = CreateState(delay: 3);
        var current = CreateState(delay: 3);

        var change = _detector.Detect(previous, current);

        Assert.Null(change);
    }

    [Fact]
    public void PlatformChange_ShouldCreateChange()
    {
        var previous = CreateState(platform: "1");
        var current = CreateState(platform: "2");

        var change = _detector.Detect(previous, current);

        Assert.NotNull(change);
        Assert.Equal("1", change.PreviousPlatform);
        Assert.Equal("2", change.CurrentPlatform);
    }

    [Fact]
    public void RunningChange_ShouldCreateChange()
    {
        var previous = CreateState(running: true);
        var current = CreateState(running: false);

        var change = _detector.Detect(previous, current);

        Assert.NotNull(change);
        Assert.True(change.PreviousRunning);
        Assert.False(change.CurrentRunning);
    }

    [Fact]
    public void NotDepartedChange_ShouldCreateChange()
    {
        var previous = CreateState(notDeparted: true);
        var current = CreateState(notDeparted: false);

        var change = _detector.Detect(previous, current);

        Assert.NotNull(change);
        Assert.True(change.PreviousNotDeparted);
        Assert.False(change.CurrentNotDeparted);
    }
}