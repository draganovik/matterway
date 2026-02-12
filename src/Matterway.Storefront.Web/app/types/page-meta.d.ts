import "nuxt/schema";

declare module "nuxt/schema" {
  interface PageMeta {
    title?: string;
    public?: boolean;
  }
}

export {};
