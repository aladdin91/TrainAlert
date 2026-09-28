using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class TrainStateStore
{
    private readonly Dictionary<string, TrainState> _states = new();

    public TrainState? Get(string trainNumber)
    {
        _states.TryGetValue(trainNumber, out var state);

        return state;
    }

    public void Set(TrainState state)
    {
        _states[state.TrainNumber] = state;
    }
}