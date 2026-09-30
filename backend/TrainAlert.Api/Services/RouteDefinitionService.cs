using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class RouteDefinitionService
{
  public RouteDefinition GetRoute(
      string originStationId,
      string destinationName)
  {
    if (
        originStationId == "S01511" &&
        destinationName.Equals(
            "MILANO PORTA GARIBALDI",
            StringComparison.OrdinalIgnoreCase))
    {
      return new RouteDefinition
      {
        OriginStationId = "S01511",
        OriginStationName = "CARNATE USMATE",

        DestinationStationName =
              "MILANO PORTA GARIBALDI",

        Stations =
          [
              "CARNATE USMATE",
                    "VIMERCATE",
                    "ARCORE",
                    "MONZA",
                    "SESTO SAN GIOVANNI",
                    "MILANO GRECO PIRELLI",
                    "MILANO PORTA GARIBALDI"
          ]
      };
    }

    return new RouteDefinition
    {
      OriginStationId = originStationId,

      OriginStationName =
            originStationId,

      DestinationStationName =
            destinationName,

      Stations =
        [
            originStationId,
                destinationName
        ]
    };
  }
}