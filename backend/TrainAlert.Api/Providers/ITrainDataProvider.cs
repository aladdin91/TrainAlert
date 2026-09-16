using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public interface ITrainDataProvider
{
    Task<List<Train>> GetTrainsAsync();
}