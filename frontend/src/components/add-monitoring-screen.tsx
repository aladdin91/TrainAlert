import { useState } from "react";
import {
  ActivityIndicator,
  FlatList,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from "react-native";

import { searchStations, type Station } from "@/api/stations";

type SelectedStation = {
  id: string;
  name: string;
};

export default function AddMonitoringScreen() {
  const [originQuery, setOriginQuery] = useState("");
  const [destinationQuery, setDestinationQuery] = useState("");

  const [originResults, setOriginResults] = useState<Station[]>([]);
  const [destinationResults, setDestinationResults] = useState<Station[]>([]);

  const [origin, setOrigin] = useState<SelectedStation | null>(null);
  const [destination, setDestination] = useState<SelectedStation | null>(null);

  const [isSearchingOrigin, setIsSearchingOrigin] = useState(false);
  const [isSearchingDestination, setIsSearchingDestination] = useState(false);

  const [error, setError] = useState("");

  async function handleOriginSearch() {
    const query = originQuery.trim();

    if (query.length < 2) {
      setOriginResults([]);
      return;
    }

    try {
      setError("");
      setIsSearchingOrigin(true);

      const results = await searchStations(query);

      setOriginResults(results);
    } catch (error) {
      console.error("Origin station search failed:", error);

      setError(
        error instanceof Error ? error.message : "Unable to search stations.",
      );
    } finally {
      setIsSearchingOrigin(false);
    }
  }

  async function handleDestinationSearch() {
    const query = destinationQuery.trim();

    if (query.length < 2) {
      setDestinationResults([]);
      return;
    }

    try {
      setError("");
      setIsSearchingDestination(true);

      const results = await searchStations(query);

      setDestinationResults(results);
    } catch (error) {
      console.error("Destination station search failed:", error);

      setError(
        error instanceof Error ? error.message : "Unable to search stations.",
      );
    } finally {
      setIsSearchingDestination(false);
    }
  }

  function selectOrigin(station: Station) {
    setOrigin({
      id: station.stationId,
      name: station.longName,
    });

    setOriginQuery(station.longName);
    setOriginResults([]);
  }

  function selectDestination(station: Station) {
    setDestination({
      id: station.stationId,
      name: station.longName,
    });

    setDestinationQuery(station.longName);
    setDestinationResults([]);
  }

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Add monitoring</Text>

      <Text style={styles.sectionTitle}>From</Text>

      <TextInput
        style={styles.input}
        placeholder="Search origin station"
        placeholderTextColor="#888"
        value={originQuery}
        onChangeText={(value) => {
          setOriginQuery(value);
          setOrigin(null);
        }}
        onSubmitEditing={handleOriginSearch}
        returnKeyType="search"
        autoCapitalize="none"
      />

      {isSearchingOrigin && <ActivityIndicator style={styles.loader} />}

      {originResults.length > 0 && (
        <View style={styles.results}>
          {originResults.map((station) => (
            <Pressable
              key={station.stationId}
              style={styles.result}
              onPress={() => selectOrigin(station)}
            >
              <Text style={styles.resultTitle}>{station.longName}</Text>

              <Text style={styles.resultSubtitle}>{station.shortName}</Text>
            </Pressable>
          ))}
        </View>
      )}

      <Pressable
        style={styles.searchButton}
        onPress={handleOriginSearch}
        disabled={isSearchingOrigin}
      >
        <Text style={styles.searchButtonText}>Search origin</Text>
      </Pressable>

      {origin && <Text style={styles.selected}>Selected: {origin.name}</Text>}

      <Text style={styles.sectionTitle}>To</Text>

      <TextInput
        style={styles.input}
        placeholder="Search destination station"
        placeholderTextColor="#888"
        value={destinationQuery}
        onChangeText={(value) => {
          setDestinationQuery(value);
          setDestination(null);
        }}
        onSubmitEditing={handleDestinationSearch}
        returnKeyType="search"
        autoCapitalize="none"
      />

      {isSearchingDestination && <ActivityIndicator style={styles.loader} />}

      {destinationResults.length > 0 && (
        <View style={styles.results}>
          {destinationResults.map((station) => (
            <Pressable
              key={station.stationId}
              style={styles.result}
              onPress={() => selectDestination(station)}
            >
              <Text style={styles.resultTitle}>{station.longName}</Text>

              <Text style={styles.resultSubtitle}>{station.shortName}</Text>
            </Pressable>
          ))}
        </View>
      )}

      <Pressable
        style={styles.searchButton}
        onPress={handleDestinationSearch}
        disabled={isSearchingDestination}
      >
        <Text style={styles.searchButtonText}>Search destination</Text>
      </Pressable>

      {destination && (
        <Text style={styles.selected}>Selected: {destination.name}</Text>
      )}

      {error ? <Text style={styles.error}>{error}</Text> : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    backgroundColor: "#fff",
  },
  title: {
    fontSize: 28,
    fontWeight: "700",
    marginBottom: 28,
  },
  sectionTitle: {
    fontSize: 17,
    fontWeight: "600",
    marginBottom: 8,
    marginTop: 18,
  },
  input: {
    height: 50,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 12,
    paddingHorizontal: 14,
    fontSize: 16,
    color: "#111",
  },
  loader: {
    marginVertical: 12,
  },
  results: {
    marginTop: 8,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 12,
    overflow: "hidden",
  },
  result: {
    padding: 14,
    borderBottomWidth: 1,
    borderBottomColor: "#eee",
  },
  resultTitle: {
    fontSize: 16,
    fontWeight: "600",
    color: "#111",
  },
  resultSubtitle: {
    marginTop: 3,
    fontSize: 13,
    color: "#777",
  },
  searchButton: {
    marginTop: 10,
    height: 44,
    borderRadius: 10,
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "#208AEF",
  },
  searchButtonText: {
    color: "#fff",
    fontSize: 15,
    fontWeight: "600",
  },
  selected: {
    marginTop: 10,
    fontSize: 14,
    color: "#208AEF",
  },
  error: {
    marginTop: 20,
    color: "#d00",
  },
});
