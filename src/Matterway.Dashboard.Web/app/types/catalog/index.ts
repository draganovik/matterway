export type { PaginationMeta, PaginationResponse } from "../common/pagination"
export type {
  ArticleDetailProperty,
  ArticleDiscountProperty,
  ArticleImageProperty,
  NumberInput,
} from "./shared"
export type {
  AddArticleImageRequest,
  ArticleImagesMutationResponse,
  CreateArticleRequest,
  CreateArticleResponse,
  DeleteArticleResponse,
  GetArticleResponse,
  PutArticleDetailRequest,
  QueryArticleResponse,
  QueryArticlesParams,
  QueryArticlesResponse,
  UpdateArticleImageRequest,
  UpdateArticleRequest,
  UpdateArticleResponse,
} from "./articles"
export type {
  DeleteDetailResponse,
  PutDetailRequest,
  PutDetailResponse,
  QueryDetailResponse,
  QueryDetailsParams,
  QueryDetailsResponse,
} from "./details"
export type {
  CreateDiscountRequest,
  DeleteDiscountResponse,
  QueryDiscountResponse,
  UpdateDiscountRequest,
  UpdatedDiscountResponse,
} from "./discounts"
export type {
  ExportCatalogArchiveResponse,
  ImportCatalogArchiveResponse,
} from "./archive"
