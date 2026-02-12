export type ApiService = "catalog" | "customers" | "identity" | "sales";

export type ApiResult<T> = {
  ok: boolean;
  status: number;
  data?: T;
  error?: string;
  validationErrors?: Record<string, string[]>;
};

export type PaginationMeta = {
  currentPage?: number;
  pageSize?: number;
  totalPages?: number;
  totalCount?: number;
};

export type PaginatedPayload<T> = {
  data?: T[];
  meta?: PaginationMeta;
  links?: Record<string, string | null>;
};
