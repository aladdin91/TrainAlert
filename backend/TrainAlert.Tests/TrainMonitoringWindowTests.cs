using TrainAlert.Api.Models;
using TrainAlert.Api.Workers;

namespace TrainAlert.Tests;

public class TrainMonitoringWindowTests
{
    [Fact]
    public void NormalWindow_TimeInside_IsProcessed()
    {
        var alert = CreateAlert("05:00", "07:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(6, 0));

        Assert.True(result);
    }

    [Fact]
    public void NormalWindow_TimeBeforeStart_IsNotProcessed()
    {
        var alert = CreateAlert("05:00", "07:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(4, 59));

        Assert.False(result);
    }

    [Fact]
    public void NormalWindow_StartBoundary_IsProcessed()
    {
        var alert = CreateAlert("05:00", "07:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(5, 0));

        Assert.True(result);
    }

    [Fact]
    public void NormalWindow_EndBoundary_IsNotProcessed()
    {
        var alert = CreateAlert("05:00", "07:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(7, 0));

        Assert.False(result);
    }

    [Fact]
    public void MidnightCrossingWindow_TimeAfterMidnight_IsProcessed()
    {
        var alert = CreateAlert("23:00", "01:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(0, 30));

        Assert.True(result);
    }

    [Fact]
    public void MidnightCrossingWindow_TimeBeforeMidnight_IsProcessed()
    {
        var alert = CreateAlert("23:00", "01:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(23, 30));

        Assert.True(result);
    }

    [Fact]
    public void MidnightCrossingWindow_TimeOutside_IsNotProcessed()
    {
        var alert = CreateAlert("23:00", "01:00");

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(12, 0));

        Assert.False(result);
    }

    [Fact]
    public void DisabledAlert_IsNotProcessed()
    {
        var alert = CreateAlert("05:00", "07:00", isEnabled: false);

        var result = TrainMonitoringWorker.ShouldProcessAlert(
            alert,
            new TimeOnly(6, 0));

        Assert.False(result);
    }

    private static AlertConfiguration CreateAlert(
        string startTime,
        string endTime,
        bool isEnabled = true)
    {
        return new AlertConfiguration
        {
            StartTime = TimeOnly.Parse(startTime),
            EndTime = TimeOnly.Parse(endTime),
            IsEnabled = isEnabled
        };
    }
}
