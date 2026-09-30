using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TrainAlert.Api.Controllers;
using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;

namespace TrainAlert.Tests;

public class StationSearchTests
{
    [Fact]
    public async Task SearchStationsAsync_MapsViaggiaTrenoStationDto()
    {
        var handler = new StubHttpMessageHandler(
            """
            [{
              "id": "S00001",
              "nomeLungo": "Example Long Station",
              "nomeBreve": "Example Short",
              "label": "Example Label"
            }]
            """);
        var provider = new ViaggiaTrenoProvider(
            new StubHttpClientFactory(handler));

        var stations = await provider.SearchStationsAsync("Example Station");

        var station = Assert.Single(stations);
        Assert.Equal("S00001", station.StationId);
        Assert.Equal("Example Long Station", station.LongName);
        Assert.Equal("Example Short", station.ShortName);
        Assert.Equal("Example Label", station.Label);
        Assert.Equal("Example Label", station.DisplayName);
    }

    [Fact]
    public async Task Search_WhitespaceQuery_ReturnsBadRequest()
    {
        var provider = new FakeStationSearchProvider();
        var controller = new StationsController(
            new StationSearchService(provider),
            new TrainService(
                new EmptyTrainDataProvider(),
                new TrainDestinationFilter()));

        var result = await controller.Search("   ");

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.False(provider.WasCalled);
    }

    [Fact]
    public async Task GetDepartures_InvalidStationId_ReturnsBadRequest()
    {
        var controller = new StationsController(
            new StationSearchService(new FakeStationSearchProvider()),
            new TrainService(
                new EmptyTrainDataProvider(),
                new TrainDestinationFilter()));

        var result = await controller.GetDepartures("invalid", null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public StubHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler);
        }
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
                Content = new StringContent(
                    _content,
                    Encoding.UTF8,
                    "application/json")
            });
        }
    }

    private sealed class FakeStationSearchProvider : IStationSearchProvider
    {
        public bool WasCalled { get; private set; }

        public Task<List<Station>> SearchStationsAsync(string query)
        {
            WasCalled = true;
            return Task.FromResult(new List<Station>());
        }
    }

    private sealed class EmptyTrainDataProvider : ITrainDataProvider
    {
        public Task<List<Train>> GetTrainsAsync(string stationId) =>
            Task.FromResult(new List<Train>());

        public Task<List<Train>> GetArrivalsAsync(string stationId) =>
            Task.FromResult(new List<Train>());

        public Task<TrainDetails> GetTrainDetailsAsync(
            string originStationId,
            string trainNumber,
            long departureDateEpochMilliseconds) =>
            Task.FromResult(new TrainDetails());

        public Task<List<TrainStop>> GetTrainStopsAsync(
            string originStationId,
            string trainNumber,
            long departureDateEpochMilliseconds) =>
            Task.FromResult(new List<TrainStop>());
    }
}
