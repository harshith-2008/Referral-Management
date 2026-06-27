import { getMe } from "../api/authApi";
import type { UserProfile } from "../types/profile";

const CACHE_KEY = "currentUser";
const CACHE_TOKEN_KEY = "currentUserToken";

let cachedUser: UserProfile | null = null;
let inFlightRequest: Promise<UserProfile> | null = null;

const getStoredUser = (token: string | null) => {
  if (!token || localStorage.getItem(CACHE_TOKEN_KEY) !== token) {
    localStorage.removeItem(CACHE_KEY);
    localStorage.removeItem(CACHE_TOKEN_KEY);
    return null;
  }

  const stored = localStorage.getItem(CACHE_KEY);
  if (!stored) return null;

  try {
    return JSON.parse(stored) as UserProfile;
  } catch {
    localStorage.removeItem(CACHE_KEY);
    localStorage.removeItem(CACHE_TOKEN_KEY);
    return null;
  }
};

export const getCurrentUser = async () => {
  const token = localStorage.getItem("token");

  if (cachedUser) return cachedUser;

  const storedUser = getStoredUser(token);
  if (storedUser) {
    cachedUser = storedUser;
    return storedUser;
  }

  if (inFlightRequest) return inFlightRequest;

  inFlightRequest = getMe()
    .then((response) => {
      cachedUser = response.data.data;

      if (token) {
        localStorage.setItem(CACHE_TOKEN_KEY, token);
        localStorage.setItem(CACHE_KEY, JSON.stringify(cachedUser));
      }

      return cachedUser;
    })
    .finally(() => {
      inFlightRequest = null;
    });

  return inFlightRequest;
};

export const clearCurrentUser = () => {
  cachedUser = null;
  inFlightRequest = null;
  localStorage.removeItem(CACHE_KEY);
  localStorage.removeItem(CACHE_TOKEN_KEY);
};
