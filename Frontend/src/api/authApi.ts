import api from "./axios";

import type { LoginDTO, LoginResponseDTO } from "../types/auth";
import type { ApiResponseDTO } from "../types/common";
import type { UserProfile } from "../types/profile";

export const login = (data: LoginDTO) =>
  api.post<ApiResponseDTO<LoginResponseDTO>>("/auth/login", data);

export const getMe = () => api.get<ApiResponseDTO<UserProfile>>("/auth/me");

export const logout = () => {
  localStorage.removeItem("token");
};
