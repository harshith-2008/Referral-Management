import { jwtDecode } from "jwt-decode";

type DecodedToken = {
  [key: string]: any;
  exp: number;
};

const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

export function getToken(): string | null {
  return localStorage.getItem("token");
}

export function getUserRole(): string | null {
  const token = getToken();
  if (!token) return null;

  try {
    const decoded = jwtDecode<DecodedToken>(token);

    return decoded[ROLE_CLAIM] || null;
  } catch {
    return null;
  }
}

export function isAuthenticated(): boolean {
  const token = getToken();
  if (!token) return false;

  try {
    const decoded = jwtDecode<DecodedToken>(token);

    return decoded.exp * 1000 > Date.now();
  } catch {
    return false;
  }
}
