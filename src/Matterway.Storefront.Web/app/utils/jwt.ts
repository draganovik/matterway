export type JwtPayload = Record<string, unknown>;

function base64UrlDecode(value: string): string {
  const normalized = value.replace(/-/g, "+").replace(/_/g, "/");
  const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, "=");
  return atob(padded);
}

export function decodeJwtPayload(token: string): JwtPayload | null {
  const parts = token.split(".");
  if (parts.length < 2) return null;
  try {
    const json = base64UrlDecode(parts[1] ?? "");
    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

export function getJwtArrayClaim(
  payload: JwtPayload | null,
  key: string,
): string[] {
  if (!payload) return [];
  const raw = payload[key];
  if (Array.isArray(raw))
    return raw.filter((item): item is string => typeof item === "string");
  if (typeof raw === "string") return [raw];
  return [];
}

export function getJwtStringClaim(
  payload: JwtPayload | null,
  key: string,
): string | null {
  if (!payload) return null;
  const raw = payload[key];
  return typeof raw === "string" ? raw : null;
}
