using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class TrainStateStore
{
    private readonly Dictionary<string, TrainState> _states = new();

    public TrainState? Get(Guid alertId, string trainNumber)
    {
        var key = $"{alertId}:{trainNumber}";

        _states.TryGetValue(key, out var state);

        return state;
    }

    public void Set(Guid alertId, TrainState state)
    {
        var key = $"{alertId}:{state.TrainNumber}";

        _states[key] = state;
    }
}