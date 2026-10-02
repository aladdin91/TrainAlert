import { apiRequest } from "./client";
import type { Train } from "../types/train";

export function getDepartures(
  stationId: string,
  destinationStationId?: string,
): Promise<Train[]> {
  const params = destinationStationId
    ? `?destinationStationId=${encodeURIComponent(destinationStationId)}`
    : "";

  return apiRequest<Train[]>(
    `/Stations/${encodeURIComponent(stationId)}/departures${params}`,
  );
}
