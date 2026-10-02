import { useCallback, useEffect, useState } from "react";
import {
  ActivityIndicator,
  Alert,
  FlatList,
  Pressable,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from "react-native";

import { deleteAlert, getAlerts } from "@/api/alerts";
import { useAuth } from "@/context/AuthContext";
import type { AlertConfiguration } from "@/types/alert";

type MonitoringScreenProps = {
  onAddMonitoring: () => void;
};

export default function MonitoringScreen({
  onAddMonitoring,
}: MonitoringScreenProps) {
  const { token } = useAuth();

  const [alerts, setAlerts] = useState<AlertConfiguration[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [error, setError] = useState("");

  const loadAlerts = useCallback(
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

        const result = await getAlerts(token);
        setAlerts(result);
      } catch (error) {
        setError(
          error instanceof Error
            ? error.message
            : "Unable to load monitoring routes.",
        );
      } finally {
        setIsLoading(false);
        setIsRefreshing(false);
      }
    },
    [token],
  );

  useEffect(() => {
    loadAlerts();
  }, [loadAlerts]);

  async function handleDelete(alert: AlertConfiguration) {
    if (!token) {
      return;
    }

    Alert.alert(
      "Delete monitoring",
      `Stop monitoring ${alert.originStationName} → ${alert.destinationStationName}?`,
      [
        {
          text: "Cancel",
          style: "cancel",
        },
        {
          text: "Delete",
          style: "destructive",
          onPress: async () => {
            try {
              await deleteAlert(token, alert.id);

              setAlerts((current) =>
                current.filter((item) => item.id !== alert.id),
              );
            } catch (error) {
              Alert.alert(
                "Unable to delete",
                error instanceof Error
                  ? error.message
                  : "Something went wrong.",
              );
            }
          },
        },
      ],
    );
  }

  function renderAlert({ item }: { item: AlertConfiguration }) {
    return (
      <View style={styles.card}>
        <View style={styles.cardHeader}>
          <View style={styles.route}>
            <Text style={styles.station}>{item.originStationName}</Text>

            <Text style={styles.arrow}>↓</Text>

            <Text style={styles.station}>{item.destinationStationName}</Text>
          </View>

          <View
            style={[
              styles.statusBadge,
              item.isEnabled ? styles.enabledBadge : styles.disabledBadge,
            ]}
          >
            <Text
              style={[
                styles.statusText,
                item.isEnabled ? styles.enabledText : styles.disabledText,
              ]}
            >
              {item.isEnabled ? "ON" : "OFF"}
            </Text>
          </View>
        </View>

        <Text style={styles.time}>
          {item.startTime} — {item.endTime}
        </Text>

        <Pressable
          style={({ pressed }) => [
            styles.deleteButton,
            pressed && styles.pressed,
          ]}
          onPress={() => handleDelete(item)}
        >
          <Text style={styles.deleteText}>Delete monitoring</Text>
        </Pressable>
      </View>
    );
  }

  if (isLoading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
        <Text style={styles.loadingText}>
          Loading your monitoring routes...
        </Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View>
          <Text style={styles.title}>Monitoring</Text>
          <Text style={styles.subtitle}>
            Routes you are currently monitoring
          </Text>
        </View>
      </View>

      {error ? (
        <View style={styles.errorBox}>
          <Text style={styles.errorText}>{error}</Text>

          <Pressable style={styles.retryButton} onPress={() => loadAlerts()}>
            <Text style={styles.retryText}>Try again</Text>
          </Pressable>
        </View>
      ) : null}

      {!error && alerts.length === 0 ? (
        <View style={styles.empty}>
          <Text style={styles.emptyTitle}>No monitored routes</Text>

          <Text style={styles.emptyText}>
            Add a route to start receiving train alerts.
          </Text>

          <Pressable style={styles.addButton} onPress={onAddMonitoring}>
            <Text style={styles.addButtonText}>Add monitoring</Text>
          </Pressable>
        </View>
      ) : null}

      {!error && alerts.length > 0 ? (
        <FlatList
          data={alerts}
          keyExtractor={(item) => item.id}
          renderItem={renderAlert}
          contentContainerStyle={styles.list}
          refreshControl={
            <RefreshControl
              refreshing={isRefreshing}
              onRefresh={() => loadAlerts(true)}
            />
          }
          ListFooterComponent={
            <Pressable style={styles.addButton} onPress={onAddMonitoring}>
              <Text style={styles.addButtonText}>+ Add monitoring</Text>
            </Pressable>
          }
        />
      ) : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
    paddingTop: 64,
  },
  header: {
    paddingHorizontal: 20,
    marginBottom: 20,
  },
  title: {
    fontSize: 32,
    fontWeight: "700",
  },
  subtitle: {
    marginTop: 6,
    fontSize: 15,
    color: "#666",
  },
  list: {
    paddingHorizontal: 20,
    paddingBottom: 32,
    gap: 14,
  },
  card: {
    borderWidth: 1,
    borderColor: "#e5e5e5",
    borderRadius: 16,
    padding: 16,
  },
  cardHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    gap: 12,
  },
  route: {
    flex: 1,
  },
  station: {
    fontSize: 18,
    fontWeight: "600",
  },
  arrow: {
    fontSize: 18,
    color: "#888",
    marginVertical: 3,
  },
  statusBadge: {
    alignSelf: "flex-start",
    borderRadius: 8,
    paddingHorizontal: 9,
    paddingVertical: 5,
  },
  enabledBadge: {
    backgroundColor: "#e8f7ee",
  },
  disabledBadge: {
    backgroundColor: "#f1f1f1",
  },
  statusText: {
    fontSize: 12,
    fontWeight: "700",
  },
  enabledText: {
    color: "#16803c",
  },
  disabledText: {
    color: "#666",
  },
  time: {
    marginTop: 14,
    color: "#666",
    fontSize: 14,
  },
  deleteButton: {
    marginTop: 16,
    paddingVertical: 8,
  },
  deleteText: {
    color: "#d32f2f",
    fontSize: 14,
    fontWeight: "500",
  },
  pressed: {
    opacity: 0.6,
  },
  addButton: {
    minHeight: 50,
    borderRadius: 12,
    backgroundColor: "#208AEF",
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: 20,
    marginTop: 8,
  },
  addButtonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "600",
  },
  center: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    padding: 24,
  },
  loadingText: {
    marginTop: 12,
    color: "#666",
  },
  empty: {
    flex: 1,
    alignItems: "center",
    justifyContent: "center",
    padding: 30,
  },
  emptyTitle: {
    fontSize: 22,
    fontWeight: "700",
  },
  emptyText: {
    marginTop: 8,
    color: "#666",
    textAlign: "center",
    lineHeight: 21,
  },
  errorBox: {
    marginHorizontal: 20,
    padding: 16,
    borderRadius: 12,
    backgroundColor: "#fff0f0",
  },
  errorText: {
    color: "#b42318",
  },
  retryButton: {
    marginTop: 12,
  },
  retryText: {
    color: "#b42318",
    fontWeight: "600",
  },
});
