import type { EmptyQuery } from "../contracts/types"
import { isRecord } from "./parsers"

export function validateNoQueryParams(data: unknown): EmptyQuery {
  if (!isRecord(data)) {
    return {}
  }

  if (Object.keys(data).length > 0) {
    throw new Error("Query parameters are not supported for this endpoint.")
  }

  return {}
}
