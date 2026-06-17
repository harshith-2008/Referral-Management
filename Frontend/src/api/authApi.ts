import api from "./axios";
import type { LoginDTO } from "../types/auth";

export const login = (data: LoginDTO) => api.post("/auth/login", data);
