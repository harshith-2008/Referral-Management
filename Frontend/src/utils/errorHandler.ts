import axios from "axios";

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as any;

    return (
      data?.message ||
      data?.Message ||
      data?.title ||
      error.response?.statusText ||
      error.message ||
      "Something went wrong"
    );
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Something went wrong";
}
