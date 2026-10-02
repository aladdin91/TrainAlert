import MonitoringScreen from "@/components/monitoring-screen";

export default function MonitoringPage() {
  return (
    <MonitoringScreen
      onAddMonitoring={() => {
        console.log("Add monitoring pressed");
      }}
    />
  );
}
