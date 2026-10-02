import { apiRequest } from "./client";
import type { AlertConfiguration } from "../types/alert";

export type CreateAlertRequest = {
  originStationId: string;
  originStationName: string;
  destinationStationId: string;
  destinationStationName: string;
  startTime: string;
  endTime: string;
  isEnabled: boolean;
};

export function getAlerts(token: string): Promise<AlertConfiguration[]> {
  return apiRequest<AlertConfiguration[]>("/Alerts", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

export function createAlert(
  token: string,
  request: CreateAlertRequest,
): Promise<AlertConfiguration> {
  return apiRequest<AlertConfiguration>("/Alerts", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(request),
  });
}

export function deleteAlert(token: string, alertId: string): Promise<void> {
  return apiRequest<void>(`/Alerts/${alertId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}
