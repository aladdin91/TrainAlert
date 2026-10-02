import { useState } from "react";
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from "react-native";

import { useAuth } from "@/context/AuthContext";
type LoginScreenProps = {
  onCreateAccount: () => void;
};

export default function LoginScreen({ onCreateAccount }: LoginScreenProps) {
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleLogin() {
    console.log("LOGIN BUTTON PRESSED");

    setError("");

    if (!email.trim() || !password) {
      setError("Please enter your email and password.");
      return;
    }
    console.log("LOGIN VALIDATION PASSED");

    try {
      setIsSubmitting(true);

      console.log("CALLING AUTH LOGIN");

      await login(email.trim(), password);
      console.log("AUTH LOGIN SUCCESS");
    } catch (error) {
      console.log("AUTH LOGIN ERROR:", error);

      setError(error instanceof Error ? error.message : "Unable to login.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <View style={styles.container}>
      <View style={styles.content}>
        <Text style={styles.logo}>TrainAlert</Text>

        <Text style={styles.title}>Welcome back</Text>

        <Text style={styles.subtitle}>Sign in to monitor your trains.</Text>

        <TextInput
          style={styles.input}
          placeholder="Email"
          placeholderTextColor="#888"
          autoCapitalize="none"
          autoCorrect={false}
          keyboardType="email-address"
          value={email}
          onChangeText={setEmail}
        />

        <TextInput
          style={styles.input}
          placeholder="Password"
          placeholderTextColor="#888"
          secureTextEntry
          value={password}
          onChangeText={setPassword}
        />

        {error ? <Text style={styles.error}>{error}</Text> : null}

        <Pressable
          style={({ pressed }) => [
            styles.button,
            pressed && styles.buttonPressed,
            isSubmitting && styles.buttonDisabled,
          ]}
          onPress={handleLogin}
          disabled={isSubmitting}
        >
          {isSubmitting ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text style={styles.buttonText}>Sign in</Text>
          )}
        </Pressable>
        <Pressable
          style={styles.secondaryButton}
          onPress={onCreateAccount}
          disabled={isSubmitting}
        >
          <Text style={styles.secondaryButtonText}>
            Don't have an account? Create one
          </Text>
        </Pressable>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    paddingHorizontal: 24,
    backgroundColor: "#f7f8fa",
  },

  content: {
    width: "100%",
    maxWidth: 420,
    alignSelf: "center",
  },

  logo: {
    fontSize: 32,
    fontWeight: "800",
    marginBottom: 32,
  },

  title: {
    fontSize: 28,
    fontWeight: "700",
    marginBottom: 8,
  },

  subtitle: {
    fontSize: 16,
    color: "#666",
    marginBottom: 28,
  },

  input: {
    height: 52,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 12,
    backgroundColor: "#fff",
    paddingHorizontal: 16,
    fontSize: 16,
    marginBottom: 12,
  },

  error: {
    color: "#d32f2f",
    marginBottom: 12,
  },

  button: {
    height: 52,
    borderRadius: 12,
    backgroundColor: "#208AEF",
    justifyContent: "center",
    alignItems: "center",
    marginTop: 8,
  },

  buttonPressed: {
    opacity: 0.8,
  },

  buttonDisabled: {
    opacity: 0.6,
  },

  buttonText: {
    color: "#fff",
    fontSize: 16,
    fontWeight: "700",
  },
  secondaryButton: {
    alignItems: "center",
    paddingVertical: 20,
  },
  secondaryButtonText: {
    color: "#208AEF",
    fontSize: 15,
    fontWeight: "500",
  },
});
