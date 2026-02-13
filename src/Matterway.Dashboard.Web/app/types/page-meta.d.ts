import type {
  ActionKey,
  PermissionLevel,
  ServiceSection,
} from "~/types/services/definitions"

declare module "#app" {
  interface PageMeta {
    public?: boolean
    service?: ServiceSection["service"]
    level?: PermissionLevel
    action?: ActionKey
  }
}

export {}
