using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class DisruptionRouteMatcherTests
{
  private readonly DisruptionRouteMatcher _matcher = new();

  private readonly List<TrainStop> _route =
    [new() { StationName = "CARNATE USMATE" },
     new() { StationName = "MILANO GRECO PIRELLI" },
     new() { StationName = "MILANO PORTA GARIBALDI" }];

  [Fact]
  public void ActiveDisruptionMentioningRouteStation_Matches()
  {
    var disruption = new Disruption
    {
      Title = "Linea Milano - Bergamo",
      Description =
            "Circolazione interrotta a MILANO GRECO PIRELLI."
            ,
      Status = "Active",
      Scope = "Route"
    };

    var result =
        _matcher.MatchesRoute(
            disruption,
            _route);

    Assert.True(result);
  }

  [Fact]
  public void UnrelatedRouteDisruption_DoesNotMatch()
  {
    var disruption = new Disruption
    {
      Title = "Linea La Spezia - Pisa",
      Description =
            "Circolazione rallentata.",
      Status = "Active",
      Scope = "Route"
    };

    var result =
        _matcher.MatchesRoute(
            disruption,
            _route);

    Assert.False(result);
  }

  [Fact]
  public void TrainDisruption_DoesNotMatchRoute()
  {
    var disruption = new Disruption
    {
      Title = "INFOTRENI FRECCE",
      Description =
            "Ritardi sulla rete.",
      Status = "Active",
      Scope = "Train"
    };

    var result =
        _matcher.MatchesRoute(
            disruption,
            _route);

    Assert.False(result);
  }

  [Fact]
  public void ResolvedDisruption_DoesNotMatch()
  {
    var disruption = new Disruption
    {
      Title = "Linea Milano - Bergamo",
      Description =
            "Circolazione regolare.",
      Status = "Resolved",
      Scope = "Route"
    };

    var result =
        _matcher.MatchesRoute(
            disruption,
            _route);

    Assert.False(result);
  }
}
