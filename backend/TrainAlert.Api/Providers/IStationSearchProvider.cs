using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public interface IStationSearchProvider
{
    Task<List<Station>> SearchStationsAsync(string query);
}
