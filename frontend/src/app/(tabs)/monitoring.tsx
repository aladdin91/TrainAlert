import { router } from "expo-router";

import MonitoringScreen from "@/components/monitoring-screen";

export default function MonitoringPage() {
  return (
    <MonitoringScreen
      onAddMonitoring={() => {
        console.log("ADD MONITORING PRESSED");

        router.push({
          pathname: "/add-monitoring",
        });
      }}
    />
  );
}
