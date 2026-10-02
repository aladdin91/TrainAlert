import { useCallback, useEffect, useState } from "react";
import {
  ActivityIndicator,
  Pressable,
  RefreshControl,
  SafeAreaView,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from "react-native";

import { getAlerts } from "@/api/alerts";
import { getDepartures } from "@/api/stations";
import { useAuth } from "@/context/AuthContext";
import type { AlertConfiguration } from "@/types/alert";
import type { Train } from "@/types/train";

function formatTime(value: string) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "--:--";
  }

  return date.toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getTrainStatus(train: Train) {
  if (train.cancelled) {
    return {
      label: "Cancelled",
      type: "cancelled" as const,
    };
  }

  if (train.delayMinutes >= 5) {
    return {
      label: `+${train.delayMinutes} min`,
      type: "delayed" as const,
    };
  }

  return {
    label: "On time",
    type: "onTime" as const,
  };
}

function getRouteStatus(trains: Train[]) {
  if (trains.some((train) => train.cancelled)) {
    return {
      label: "Service disruption",
      type: "disrupted" as const,
    };
  }

  if (trains.some((train) => train.delayMinutes >= 5)) {
    return {
      label: "Some trains delayed",
      type: "delayed" as const,
    };
  }

  return {
    label: "Service normal",
    type: "normal" as const,
  };
}

export default function HomeScreen() {
  const { user, token, logout } = useAuth();

  const [alerts, setAlerts] = useState<AlertConfiguration[]>([]);
  const [trains, setTrains] = useState<Train[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [error, setError] = useState("");

  const loadHome = useCallback(
    async (refresh = false) => {
      if (!token) {
        return;
      }

      try {
        setError("");

        if (refresh) {
          setIsRefreshing(true);
        } else {
          setIsLoading(true);
        }

        const userAlerts = await getAlerts(token);
        setAlerts(userAlerts);

        const activeAlert = userAlerts.find((alert) => alert.isEnabled);

        if (!activeAlert) {
          setTrains([]);
          return;
        }

        const departures = await getDepartures(
          activeAlert.originStationId,
          activeAlert.destinationStationId,
        );

        setTrains(departures);
      } catch (error) {
        console.error("Failed to load home:", error);

        setError(
          error instanceof Error
            ? error.message
            : "Unable to load train information.",
        );
      } finally {
        setIsLoading(false);
        setIsRefreshing(false);
      }
    },
    [token],
  );

  useEffect(() => {
    loadHome();
  }, [loadHome]);

  if (isLoading) {
    return (
      <SafeAreaView style={styles.loadingContainer}>
        <ActivityIndicator size="large" />
        <Text style={styles.loadingText}>Loading your trains...</Text>
      </SafeAreaView>
    );
  }

  const activeAlert = alerts.find((alert) => alert.isEnabled);
  const routeStatus = getRouteStatus(trains);

  return (
    <SafeAreaView style={styles.container}>
      <ScrollView
        contentContainerStyle={styles.content}
        refreshControl={
          <RefreshControl
            refreshing={isRefreshing}
            onRefresh={() => loadHome(true)}
          />
        }
      >
        <View style={styles.header}>
          <View>
            <Text style={styles.greeting}>Good evening</Text>
            <Text style={styles.email}>{user?.email}</Text>
          </View>

          <Pressable onPress={logout} style={styles.logoutButton}>
            <Text style={styles.logoutText}>Log out</Text>
          </Pressable>
        </View>

        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Monitored route</Text>

          <Text style={styles.alertCount}>
            {alerts.length} alert{alerts.length === 1 ? "" : "s"}
          </Text>
        </View>

        {!activeAlert ? (
          <View style={styles.emptyCard}>
            <Text style={styles.emptyTitle}>No monitored routes</Text>

            <Text style={styles.emptyText}>
              Add a route to start monitoring your trains.
            </Text>
          </View>
        ) : (
          <>
            <View style={styles.routeCard}>
              <View style={styles.routeHeader}>
                <View style={styles.routeDot} />

                <Text
                  style={[
                    styles.routeStatus,
                    routeStatus.type === "delayed" && styles.routeStatusDelayed,
                    routeStatus.type === "disrupted" &&
                      styles.routeStatusDisrupted,
                  ]}
                >
                  {routeStatus.label}
                </Text>
              </View>

              <Text style={styles.stationName}>
                {activeAlert.originStationName}
              </Text>

              <View style={styles.routeArrow}>
                <View style={styles.routeLine} />
                <Text style={styles.arrow}>↓</Text>
                <View style={styles.routeLine} />
              </View>

              <Text style={styles.stationName}>
                {activeAlert.destinationStationName}
              </Text>

              <View style={styles.monitoringTime}>
                <Text style={styles.monitoringLabel}>Monitoring</Text>

                <Text style={styles.monitoringValue}>
                  {activeAlert.startTime} – {activeAlert.endTime}
                </Text>
              </View>
            </View>

            <View style={styles.sectionHeader}>
              <Text style={styles.sectionTitle}>Next trains</Text>

              <Text style={styles.trainCount}>{trains.length}</Text>
            </View>

            {error ? (
              <View style={styles.errorCard}>
                <Text style={styles.errorTitle}>Unable to load trains</Text>

                <Text style={styles.errorText}>{error}</Text>

                <Pressable
                  onPress={() => loadHome()}
                  style={styles.retryButton}
                >
                  <Text style={styles.retryText}>Try again</Text>
                </Pressable>
              </View>
            ) : trains.length === 0 ? (
              <View style={styles.emptyCard}>
                <Text style={styles.emptyTitle}>No trains found</Text>

                <Text style={styles.emptyText}>
                  There are no matching departures right now.
                </Text>
              </View>
            ) : (
              trains.slice(0, 6).map((train) => {
                const status = getTrainStatus(train);

                return (
                  <Pressable
                    key={`${train.trainNumber}-${train.scheduledDeparture}`}
                    style={styles.trainCard}
                  >
                    <View style={styles.trainTimeColumn}>
                      <Text style={styles.trainTime}>
                        {formatTime(train.scheduledDeparture)}
                      </Text>

                      {train.platform ? (
                        <Text style={styles.platform}>
                          Platform {train.platform}
                        </Text>
                      ) : null}
                    </View>

                    <View style={styles.trainInfo}>
                      <Text style={styles.trainNumber}>
                        {train.trainNumber}
                      </Text>

                      <Text style={styles.trainDestination}>
                        → {train.destination}
                      </Text>
                    </View>

                    <View
                      style={[
                        styles.statusBadge,
                        status.type === "delayed" && styles.statusBadgeDelayed,
                        status.type === "cancelled" &&
                          styles.statusBadgeCancelled,
                      ]}
                    >
                      <Text
                        style={[
                          styles.statusText,
                          status.type === "delayed" && styles.statusTextDelayed,
                          status.type === "cancelled" &&
                            styles.statusTextCancelled,
                        ]}
                      >
                        {status.label}
                      </Text>
                    </View>
                  </Pressable>
                );
              })
            )}
          </>
        )}
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#F7F8FA",
  },

  content: {
    padding: 20,
    paddingBottom: 40,
  },

  loadingContainer: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#F7F8FA",
  },

  loadingText: {
    marginTop: 12,
    color: "#666",
  },

  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 28,
  },

  greeting: {
    fontSize: 28,
    fontWeight: "700",
    color: "#111",
  },

  email: {
    marginTop: 4,
    fontSize: 14,
    color: "#707070",
  },

  logoutButton: {
    paddingVertical: 8,
    paddingHorizontal: 12,
  },

  logoutText: {
    fontSize: 14,
    fontWeight: "600",
    color: "#555",
  },

  sectionHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: 12,
  },

  sectionTitle: {
    fontSize: 19,
    fontWeight: "700",
    color: "#111",
  },

  alertCount: {
    fontSize: 13,
    color: "#777",
  },

  trainCount: {
    minWidth: 26,
    textAlign: "center",
    paddingVertical: 3,
    paddingHorizontal: 7,
    borderRadius: 12,
    backgroundColor: "#E8EAED",
    color: "#555",
    fontSize: 12,
    fontWeight: "600",
  },

  routeCard: {
    backgroundColor: "#FFFFFF",
    borderRadius: 20,
    padding: 20,
    marginBottom: 28,
    shadowOpacity: 0.05,
    shadowRadius: 10,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    elevation: 2,
  },

  routeHeader: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 20,
  },

  routeDot: {
    width: 9,
    height: 9,
    borderRadius: 5,
    backgroundColor: "#24A148",
    marginRight: 8,
  },

  routeStatus: {
    color: "#198038",
    fontSize: 14,
    fontWeight: "600",
  },

  routeStatusDelayed: {
    color: "#A15C00",
  },

  routeStatusDisrupted: {
    color: "#C62828",
  },

  stationName: {
    fontSize: 21,
    fontWeight: "700",
    color: "#111",
  },

  routeArrow: {
    flexDirection: "row",
    alignItems: "center",
    marginVertical: 8,
  },

  routeLine: {
    flex: 1,
    height: 1,
    backgroundColor: "#E0E0E0",
  },

  arrow: {
    marginHorizontal: 10,
    fontSize: 18,
    color: "#777",
  },

  monitoringTime: {
    flexDirection: "row",
    justifyContent: "space-between",
    marginTop: 20,
    paddingTop: 14,
    borderTopWidth: 1,
    borderTopColor: "#EEEEEE",
  },

  monitoringLabel: {
    color: "#777",
    fontSize: 13,
  },

  monitoringValue: {
    color: "#333",
    fontSize: 13,
    fontWeight: "600",
  },

  trainCard: {
    backgroundColor: "#FFFFFF",
    borderRadius: 16,
    padding: 16,
    marginBottom: 10,
    flexDirection: "row",
    alignItems: "center",
  },

  trainTimeColumn: {
    width: 72,
  },

  trainTime: {
    fontSize: 19,
    fontWeight: "700",
    color: "#111",
  },

  platform: {
    marginTop: 4,
    fontSize: 11,
    color: "#777",
  },

  trainInfo: {
    flex: 1,
    paddingHorizontal: 12,
  },

  trainNumber: {
    fontSize: 14,
    fontWeight: "700",
    color: "#333",
  },

  trainDestination: {
    marginTop: 4,
    fontSize: 13,
    color: "#777",
  },

  statusBadge: {
    paddingVertical: 6,
    paddingHorizontal: 9,
    borderRadius: 10,
    backgroundColor: "#E8F5E9",
  },

  statusBadgeDelayed: {
    backgroundColor: "#FFF3E0",
  },

  statusBadgeCancelled: {
    backgroundColor: "#FFEBEE",
  },

  statusText: {
    fontSize: 11,
    fontWeight: "700",
    color: "#198038",
  },

  statusTextDelayed: {
    color: "#A15C00",
  },

  statusTextCancelled: {
    color: "#C62828",
  },

  emptyCard: {
    backgroundColor: "#FFFFFF",
    borderRadius: 18,
    padding: 24,
    alignItems: "center",
  },

  emptyTitle: {
    fontSize: 17,
    fontWeight: "700",
    color: "#222",
  },

  emptyText: {
    marginTop: 8,
    fontSize: 14,
    color: "#777",
    textAlign: "center",
    lineHeight: 20,
  },

  errorCard: {
    backgroundColor: "#FFF5F5",
    borderRadius: 16,
    padding: 18,
  },

  errorTitle: {
    fontSize: 16,
    fontWeight: "700",
    color: "#B42318",
  },

  errorText: {
    marginTop: 6,
    fontSize: 13,
    color: "#7A271A",
  },

  retryButton: {
    marginTop: 14,
    alignSelf: "flex-start",
    backgroundColor: "#111",
    paddingVertical: 9,
    paddingHorizontal: 14,
    borderRadius: 9,
  },

  retryText: {
    color: "#FFF",
    fontSize: 13,
    fontWeight: "600",
  },
});
