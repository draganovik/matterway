export type { PaginationMeta, PaginationResponse } from "../common/pagination"
export type {
  ArticleDetailProperty,
  ArticleDiscountProperty,
  ArticleImageProperty,
  NumberInput,
} from "./shared"
export type {
  AddArticleDetailRequest,
  AddArticleImageRequest,
  ArticleImagesMutationResponse,
  CreateArticleRequest,
  CreateArticleResponse,
  DeleteArticleResponse,
  GetArticleResponse,
  QueryArticleResponse,
  QueryArticlesParams,
  QueryArticlesResponse,
  UpdateArticleDetailRequest,
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
  CreatedDiscountResponse,
  DeleteDiscountResponse,
  QueryDiscountResponse,
  UpdateDiscountRequest,
  UpdatedDiscountResponse,
} from "./discounts"
export type {
  ExportCatalogArchiveResponse,
  ImportCatalogArchiveResponse,
} from "./archive"
