using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;

namespace TrainAlert.Api.Services;

public class StationSearchService
{
    private readonly IStationSearchProvider _stationSearchProvider;

    public StationSearchService(IStationSearchProvider stationSearchProvider)
    {
        _stationSearchProvider = stationSearchProvider;
    }

    public Task<List<Station>> SearchAsync(string query)
    {
        return _stationSearchProvider.SearchStationsAsync(query);
    }
}
