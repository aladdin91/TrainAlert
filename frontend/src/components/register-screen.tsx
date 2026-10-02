import { useState } from "react";
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from "react-native";

import { register } from "@/api/auth";

type RegisterScreenProps = {
  onBackToLogin: () => void;
};

export default function RegisterScreen({ onBackToLogin }: RegisterScreenProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);

  async function handleRegister() {
    setError("");
    setSuccess(false);

    if (!email.trim() || !password || !confirmPassword) {
      setError("Please fill in all fields.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    try {
      setIsSubmitting(true);

      await register({
        email: email.trim(),
        password,
      });

      setSuccess(true);
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Unable to create account.",
      );
    } finally {
      setIsSubmitting(false);
    }
  }

  if (success) {
    return (
      <View style={styles.container}>
        <View style={styles.content}>
          <Text style={styles.logo}>TrainAlert</Text>
          <Text style={styles.title}>Account created</Text>
          <Text style={styles.subtitle}>
            Your account was created successfully. You can now sign in.
          </Text>

          <Pressable style={styles.button} onPress={onBackToLogin}>
            <Text style={styles.buttonText}>Go to sign in</Text>
          </Pressable>
        </View>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.content}>
        <Text style={styles.logo}>TrainAlert</Text>

        <Text style={styles.title}>Create account</Text>

        <Text style={styles.subtitle}>
          Create an account to start monitoring your trains.
        </Text>

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
          autoCapitalize="none"
          autoCorrect={false}
          textContentType="oneTimeCode"
          value={password}
          onChangeText={setPassword}
        />

        <TextInput
          style={styles.input}
          placeholder="Confirm password"
          placeholderTextColor="#888"
          secureTextEntry
          autoCapitalize="none"
          autoCorrect={false}
          textContentType="oneTimeCode"
          value={confirmPassword}
          onChangeText={setConfirmPassword}
        />

        {error ? <Text style={styles.error}>{error}</Text> : null}

        <Pressable
          style={({ pressed }) => [
            styles.button,
            pressed && styles.buttonPressed,
            isSubmitting && styles.buttonDisabled,
          ]}
          onPress={handleRegister}
          disabled={isSubmitting}
        >
          {isSubmitting ? (
            <ActivityIndicator color="#fff" />
          ) : (
            <Text style={styles.buttonText}>Create account</Text>
          )}
        </Pressable>

        <Pressable
          style={styles.secondaryButton}
          onPress={onBackToLogin}
          disabled={isSubmitting}
        >
          <Text style={styles.secondaryButtonText}>
            Already have an account? Sign in
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
    backgroundColor: "#fff",
  },
  content: {
    padding: 24,
  },
  logo: {
    fontSize: 32,
    fontWeight: "700",
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
    marginBottom: 24,
    lineHeight: 22,
  },
  input: {
    height: 52,
    borderWidth: 1,
    borderColor: "#ddd",
    borderRadius: 12,
    paddingHorizontal: 16,
    marginBottom: 12,
    fontSize: 16,
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
    fontWeight: "600",
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
