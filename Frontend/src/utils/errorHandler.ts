import axios from "axios";

interface ApiErrorResponse {
  success: boolean;
  message: string;
  data: unknown;
}

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError<ApiErrorResponse>(error)) {
    return (
      error.response?.data?.message ||
      error.response?.statusText ||
      "Request failed"
    );
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Something went wrong";
}
