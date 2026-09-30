using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public interface IInfomobilityProvider
{
  Task<List<Disruption>> GetDisruptionsAsync();
}