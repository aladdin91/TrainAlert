import { apiRequest } from "./client";
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
} from "../types/auth";

export function login(request: LoginRequest): Promise<LoginResponse> {
  return apiRequest<LoginResponse>("/Auth/login", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function register(request: RegisterRequest): Promise<RegisterResponse> {
  return apiRequest<RegisterResponse>("/Auth/register", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
