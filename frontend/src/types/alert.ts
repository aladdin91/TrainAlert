export type AlertConfiguration = {
  id: string;
  userId: string;
  originStationId: string;
  originStationName: string;
  destinationStationId: string;
  destinationStationName: string;
  startTime: string;
  endTime: string;
  isEnabled: boolean;
};
