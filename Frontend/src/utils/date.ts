const isDateOnly = (value: string) => /^\d{4}-\d{2}-\d{2}$/.test(value);

const toDate = (value?: string | null) => {
  if (!value) return null;

  const parsed = isDateOnly(value)
    ? new Date(`${value}T00:00:00`)
    : new Date(value);

  return Number.isNaN(parsed.getTime()) ? null : parsed;
};

export const formatDate = (
  value?: string | null,
  options: Intl.DateTimeFormatOptions = {
    day: "numeric",
    month: "short",
    year: "numeric",
  },
) => {
  const date = toDate(value);

  return date ? date.toLocaleDateString(undefined, options) : "—";
};

export const formatDateTime = (
  value?: string | null,
  options: Intl.DateTimeFormatOptions = {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  },
) => {
  const date = toDate(value);

  return date ? date.toLocaleString(undefined, options) : "—";
};

export const formatTime = (value?: string | null) => {
  if (!value) return "—";

  const [hour, minute] = value.split(":");

  if (!hour || !minute) return value;

  const date = new Date();
  date.setHours(Number(hour), Number(minute), 0, 0);

  if (Number.isNaN(date.getTime())) return value;

  return date.toLocaleTimeString(undefined, {
    hour: "numeric",
    minute: "2-digit",
  });
};

export const getTodayInputValue = () => {
  const today = new Date();
  const offset = today.getTimezoneOffset();
  const localDate = new Date(today.getTime() - offset * 60 * 1000);

  return localDate.toISOString().split("T")[0];
};
