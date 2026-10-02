import { apiRequest } from "./client";
import type { AlertConfiguration } from "../types/alert";

export function getAlerts(token: string): Promise<AlertConfiguration[]> {
  return apiRequest<AlertConfiguration[]>("/Alerts", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
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
