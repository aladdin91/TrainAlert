using System.Net;
using System.Text;
using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class TrainJourneyTests
{
    [Fact]
    public async Task GetTrainsAsync_MapsDepartureBoard()
    {
        var provider = CreateProvider(
            """
            [{"numeroTreno":123,"destinazione":"Destination","codOrigine":"S00001","dataPartenzaTreno":1700000000000,"orarioPartenza":1700003600000,"ritardo":4,"binarioProgrammatoPartenzaDescrizione":"3","circolante":true,"nonPartito":false,"provvedimento":1}]
            """);

        var train = Assert.Single(await provider.GetTrainsAsync("S00001"));

        Assert.Equal("123", train.TrainNumber);
        Assert.Equal("Destination", train.Destination);
        Assert.Equal("S00001", train.OriginStationId);
        Assert.Equal(4, train.DelayMinutes);
        Assert.Equal("3", train.Platform);
        Assert.Equal(1700000000000, train.DepartureDateEpochMilliseconds);
        Assert.True(train.Cancelled);
    }

    [Fact]
    public async Task GetArrivalsAsync_MapsArrivalBoard()
    {
        var provider = CreateProvider(
            """
            [{"numeroTreno":456,"origine":"Origin","codOrigine":"S00002","orarioArrivo":1700003600000,"ritardo":2,"binarioProgrammatoArrivoDescrizione":"5","circolante":true,"nonPartito":false}]
            """);

        var train = Assert.Single(await provider.GetArrivalsAsync("S00003"));

        Assert.Equal("456", train.TrainNumber);
        Assert.Equal("Origin", train.Origin);
        Assert.NotNull(train.ScheduledArrival);
        Assert.Equal("5", train.Platform);
    }

    [Fact]
    public async Task GetTrainDetailsAsync_MapsTrainDetails()
    {
        var provider = CreateProvider(DetailsJson);

        var details = await provider.GetTrainDetailsAsync(
            "S00001", "123", 1700000000000);

        Assert.Equal("123", details.TrainNumber);
        Assert.Equal("Origin", details.Origin);
        Assert.Equal("Destination", details.Destination);
        Assert.Equal("S00002", details.DestinationStationId);
        Assert.Equal("Technical issue", details.DisruptionReason);
    }

    [Fact]
    public async Task GetTrainStopsAsync_MapsStops()
    {
        var provider = CreateProvider(DetailsJson);

        var stop = Assert.Single(await provider.GetTrainStopsAsync(
            "S00001", "123", 1700000000000));

        Assert.Equal("S00002", stop.StationId);
        Assert.Equal("Destination", stop.StationName);
        Assert.Equal(2, stop.Sequence);
        Assert.Equal("4", stop.ArrivalPlatform);
        Assert.Equal("5", stop.DeparturePlatform);
        Assert.NotNull(stop.ScheduledArrival);
        Assert.NotNull(stop.ActualDeparture);
    }

    [Fact]
    public void StopsAtDestination_IncludesTrainWithSelectedFinalStop()
    {
        var filter = new TrainDestinationFilter();

        var result = filter.StopsAtDestination(
            [new TrainStop { StationId = "S00001" },
             new TrainStop { StationId = "S00002" }],
            "S00002");

        Assert.True(result);
    }

    [Fact]
    public void StopsAtDestination_IncludesTrainWithSelectedIntermediateStop()
    {
        var filter = new TrainDestinationFilter();

        var result = filter.StopsAtDestination(
            [new TrainStop { StationId = "S00001" },
             new TrainStop { StationId = "S00002" },
             new TrainStop { StationId = "S00003" }],
            "S00002");

        Assert.True(result);
    }

    [Fact]
    public void StopsAtDestination_ExcludesTrainWithoutSelectedStop()
    {
        var filter = new TrainDestinationFilter();

        var result = filter.StopsAtDestination(
            [new TrainStop { StationId = "S00001" },
             new TrainStop { StationId = "S00003" }],
            "S00002");

        Assert.False(result);
    }

    private static ViaggiaTrenoProvider CreateProvider(string json)
    {
        return new ViaggiaTrenoProvider(
            new StubHttpClientFactory(new StubHttpMessageHandler(json)));
    }

    private const string DetailsJson = """
        {"numeroTreno":123,"origine":"Origin","idOrigine":"S00001","destinazione":"Destination","idDestinazione":"S00002","orarioPartenza":1700000000000,"orarioArrivo":1700003600000,"ritardo":3,"circolante":true,"nonPartito":false,"tipoTreno":"PG","motivoRitardoPrevalente":"Technical issue","fermate":[{"id":"S00002","stazione":"Destination","arrivo_teorico":1700003600000,"partenza_teorica":1700003660000,"arrivoReale":1700003720000,"partenzaReale":1700003780000,"binarioEffettivoArrivoDescrizione":"4","binarioEffettivoPartenzaDescrizione":"5","progressivo":2}]}
        """;

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public StubHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name) => new(_handler);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _content;

        public StubHttpMessageHandler(string content)
        {
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            });
        }
    }
}
