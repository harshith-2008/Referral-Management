import api from "./axios";

import type { ApiResponseDTO } from "../types/common";
import type { CoordinatorDashboardDTO } from "../types/coordinator";

export const getDashboard = () =>
  api.get<ApiResponseDTO<CoordinatorDashboardDTO>>("/coordinator/dashboard");
