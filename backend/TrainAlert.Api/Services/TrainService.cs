using TrainAlert.Api.Models;
using TrainAlert.Api.Providers;

namespace TrainAlert.Api.Services;

public class TrainService
{
    private readonly ITrainDataProvider _trainDataProvider;

    public TrainService(ITrainDataProvider trainDataProvider)
    {
        _trainDataProvider = trainDataProvider;
    }

    public async Task<List<Train>> GetTrainsAsync()
    {
        return await _trainDataProvider.GetTrainsAsync();
    }
}