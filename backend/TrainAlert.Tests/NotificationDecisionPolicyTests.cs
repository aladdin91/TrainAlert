using Microsoft.EntityFrameworkCore;

using TrainAlert.Api.Data;
using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class NotificationDecisionServiceTests
{
  private static NotificationDecisionService CreateService(
      out ApplicationDbContext dbContext)
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    dbContext = new ApplicationDbContext(options);

    return new NotificationDecisionService(dbContext);
  }

  private static TrainChange CreateChange(
      int previousDelay = 0,
      int currentDelay = 0,
      string? previousPlatform = "1",
      string? currentPlatform = "1",
      bool previousRunning = true,
      bool currentRunning = true,
      bool previousNotDeparted = true,
      bool currentNotDeparted = true,
      bool previousCancelled = false,
      bool currentCancelled = false)
  {
    return new TrainChange
    {
      TrainNumber = "24879",
      Origin = "CARNATE USMATE",
      Destination = "MILANO PORTA GARIBALDI",

      PreviousDelayMinutes = previousDelay,
      CurrentDelayMinutes = currentDelay,

      PreviousPlatform = previousPlatform,
      CurrentPlatform = currentPlatform,

      PreviousRunning = previousRunning,
      CurrentRunning = currentRunning,

      PreviousNotDeparted = previousNotDeparted,
      CurrentNotDeparted = currentNotDeparted,

      PreviousCancelled = previousCancelled,
      CurrentCancelled = currentCancelled
    };
  }

  [Fact]
  public async Task DelayBelowFive_ShouldNotNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousDelay: 1,
        currentDelay: 2);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.False(result);
  }

  [Fact]
  public async Task DelayCrossingFive_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousDelay: 4,
        currentDelay: 5);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task DelayWithinSameThreshold_ShouldNotNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousDelay: 5,
        currentDelay: 6);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.False(result);
  }

  [Fact]
  public async Task DelayCrossingTen_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousDelay: 5,
        currentDelay: 10);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task DelayDecrease_ShouldNotNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousDelay: 10,
        currentDelay: 5);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.False(result);
  }

  [Fact]
  public async Task PlatformChange_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousPlatform: "1",
        currentPlatform: "2");

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task RunningChange_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousRunning: true,
        currentRunning: false);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task NotDepartedChange_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousNotDeparted: true,
        currentNotDeparted: false);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task CancellationChange_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousCancelled: false,
        currentCancelled: true);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task CancellationRecovery_ShouldNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange(
        previousCancelled: true,
        currentCancelled: false);

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.True(result);
  }

  [Fact]
  public async Task DisruptionReasonChangeAlone_ShouldNotNotify()
  {
    var service = CreateService(out var db);

    var change = CreateChange();

    change.PreviousDisruptionReason =
        "Circolazione rallentata";

    change.CurrentDisruptionReason =
        "Ritardo nella circolazione";

    var result = await service.ShouldNotifyAsync(
        Guid.NewGuid(),
        change);

    Assert.False(result);
  }

  [Fact]
  public async Task ExistingNotificationAtSameThreshold_ShouldNotNotifyAgain()
  {
    var service = CreateService(out var db);

    var alertId = Guid.NewGuid();

    db.Notifications.Add(new Notification
    {
      Id = Guid.NewGuid(),
      AlertId = alertId,
      UserId = Guid.NewGuid(),
      TrainNumber = "24879",
      Origin = "CARNATE USMATE",
      Destination = "MILANO PORTA GARIBALDI",
      CurrentDelayMinutes = 10,
      CreatedAt = DateTime.UtcNow
    });

    await db.SaveChangesAsync();

    var change = CreateChange(
        previousDelay: 9,
        currentDelay: 10);

    var result = await service.ShouldNotifyAsync(
        alertId,
        change);

    Assert.False(result);
  }

  [Fact]
  public async Task ExistingNotificationAtLowerThreshold_ShouldNotify()
  {
    var service = CreateService(out var db);

    var alertId = Guid.NewGuid();

    db.Notifications.Add(new Notification
    {
      Id = Guid.NewGuid(),
      AlertId = alertId,
      UserId = Guid.NewGuid(),
      TrainNumber = "24879",
      Origin = "CARNATE USMATE",
      Destination = "MILANO PORTA GARIBALDI",
      CurrentDelayMinutes = 5,
      CreatedAt = DateTime.UtcNow
    });

    await db.SaveChangesAsync();

    var change = CreateChange(
        previousDelay: 9,
        currentDelay: 10);

    var result = await service.ShouldNotifyAsync(
        alertId,
        change);

    Assert.True(result);
  }
}