import { apiRequest } from "./client";
import type { Train } from "../types/train";

export type Station = {
  stationId: string;
  longName: string;
  shortName: string;
  label: string;
  displayName: string;
};

export function searchStations(query: string): Promise<Station[]> {
  return apiRequest<Station[]>(
    `/Stations/search?query=${encodeURIComponent(query)}`,
  );
}

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
