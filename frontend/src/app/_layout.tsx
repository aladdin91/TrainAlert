import { DarkTheme, DefaultTheme, ThemeProvider } from "expo-router";
import * as SplashScreen from "expo-splash-screen";
import {
  ActivityIndicator,
  StyleSheet,
  View,
  useColorScheme,
} from "react-native";

import { AnimatedSplashOverlay } from "@/components/animated-icon";
import AppTabs from "@/components/app-tabs";
import LoginScreen from "@/components/login-screen";
import { AuthProvider, useAuth } from "@/context/AuthContext";
import { useState } from "react";
import RegisterScreen from "@/components/register-screen";

SplashScreen.preventAutoHideAsync();

function AppContent() {
  const colorScheme = useColorScheme();
  const { user, isLoading } = useAuth();
  const [showRegister, setShowRegister] = useState(false);
  if (isLoading) {
    return (
      <View style={styles.loading}>
        <ActivityIndicator size="large" />
      </View>
    );
  }

  return (
    <ThemeProvider value={colorScheme === "dark" ? DarkTheme : DefaultTheme}>
      <AnimatedSplashOverlay />

      {user ? (
        <AppTabs />
      ) : showRegister ? (
        <RegisterScreen onBackToLogin={() => setShowRegister(false)} />
      ) : (
        <LoginScreen onCreateAccount={() => setShowRegister(true)} />
      )}
    </ThemeProvider>
  );
}

export default function TabLayout() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

const styles = StyleSheet.create({
  loading: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
});
