using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class DisruptionAnalyzerTests
{
  private readonly DisruptionAnalyzer _analyzer = new();

  [Fact]
  public void ActiveRouteDisruption_IsDetected()
  {
    var disruption = new Disruption
    {
      Title = "Linea La Spezia - Pisa",
      Description = "Circolazione rallentata per inconveniente tecnico."
    };

    var result = _analyzer.Analyze(disruption);

    Assert.Equal("Active", result.Status);
    Assert.Equal("Route", result.Scope);
    Assert.True(result.IsActive);
  }

  [Fact]
  public void ResolvedDisruption_IsDetected()
  {
    var disruption = new Disruption
    {
      Title = "Linea AV Roma - Napoli",
      Description = "Circolazione regolare dalle ore 11:20."
    };

    var result = _analyzer.Analyze(disruption);

    Assert.Equal("Resolved", result.Status);
    Assert.Equal("Route", result.Scope);
    Assert.False(result.IsActive);
  }

  [Fact]
  public void TrainInformation_IsTrainScope()
  {
    var disruption = new Disruption
    {
      Title = "INFOTRENI FRECCE",
      Description = "Informazioni relative alla circolazione."
    };

    var result = _analyzer.Analyze(disruption);

    Assert.Equal("Train", result.Scope);
  }

  [Fact]
  public void FutureRouteDisruption_IsDetected()
  {
    var disruption = new Disruption
    {
      Title = "Linea Roma - Pisa: domenica 4 ottobre",
      Description = "Modifiche alla circolazione."
    };

    var result = _analyzer.Analyze(disruption);

    Assert.Equal("Future", result.Status);
    Assert.Equal("Route", result.Scope);
    Assert.False(result.IsActive);
  }

  [Fact]
  public void RegularNetworkCirculation_IsResolved()
  {
    var disruption = new Disruption
    {
      Title = "CIRCOLAZIONE REGOLARE SULLA RETE ALTA VELOCITÀ",
      Description = "La circolazione è tornata regolare."
    };

    var result = _analyzer.Analyze(disruption);

    Assert.Equal("Resolved", result.Status);
    Assert.Equal("Network", result.Scope);
    Assert.False(result.IsActive);
  }
}