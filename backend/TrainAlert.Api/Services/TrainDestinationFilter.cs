using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class TrainDestinationFilter
{
    public bool StopsAtDestination(
        IEnumerable<TrainStop> stops,
        string destinationStationId)
    {
        return stops.Any(stop =>
            string.Equals(
                stop.StationId,
                destinationStationId,
                StringComparison.OrdinalIgnoreCase));
    }
}
