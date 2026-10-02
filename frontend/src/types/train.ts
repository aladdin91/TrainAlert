export type Train = {
  trainNumber: string;
  origin: string;
  originStationId: string;
  destination: string;
  scheduledDeparture: string;
  actualDeparture?: string | null;
  scheduledArrival?: string | null;
  departureDateEpochMilliseconds?: number | null;
  delayMinutes: number;
  platform?: string | null;
  running: boolean;
  notDeparted: boolean;
  cancelled: boolean;
  disruptionReason?: string | null;
};
