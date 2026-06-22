import api from "./axios";

import type { LoginDTO, LoginResponseDTO } from "../types/auth";
import type { ApiResponseDTO } from "../types/common";

export const login = (data: LoginDTO) =>
  api.post<ApiResponseDTO<LoginResponseDTO>>("/auth/login", data);
