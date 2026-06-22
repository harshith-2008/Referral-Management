import api from "./axios";

import type { ApiResponseDTO } from "../types/common";
import type { OverviewAnalyticsDTO } from "../types/admin";

export const getOverviewAnalytics = (from?: string, to?: string) =>
  api.get<ApiResponseDTO<OverviewAnalyticsDTO>>("/admin/analytics/overview", {
    params: {
      from,
      to,
    },
  });
