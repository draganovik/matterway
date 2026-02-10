// stores/catalog.ts

import { defineStore } from "pinia";
import ArticleModel from "#models/ArticleModel";

interface CatalogState {
  catalog: ArticleModel[] | null;
  catalogMeta: any;
  catalogLinks: any;
}

export interface DetailOption {
  slug: string;
  title?: string;
  unit?: string | null;
}

export const useCatalogStore = defineStore("catalog", {
  state: (): CatalogState => ({
    catalog: null,
    catalogMeta: null,
    catalogLinks: null,
  }),

  getters: {
    getCatalogData(): ArticleModel[] | null {
      return this.catalog;
    },
    getCatalogMeta(): any {
      return this.catalogMeta;
    },
    getCatalogLinks(): any {
      return this.catalogLinks;
    },
  },

  actions: {
    async fetchArticleById(id: string): Promise<ArticleModel | null> {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.catalogApiBaseUrl}/api/v1.0/public/articles/${id}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
        },
      );
      if (response.ok) {
        try {
          const responseObject = await response.json();
          return ArticleModel.fromDetailResponse(responseObject);
        } catch (error) {
          console.error("Failed to parse article response", error);
        }
      }
      return null;
    },
    async fetchCatalog(options?: {
      page?: number;
      pageSize?: number;
      filter?: string;
    }) {
      const page = options?.page ?? 1;
      const pageSize = options?.pageSize ?? 10;
      const filter = options?.filter?.trim();

      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      if (filter) {
        params.set("filter", filter);
      }

      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.catalogApiBaseUrl}/api/v1.0/public/articles?${params.toString()}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
        },
      );

      if (response.status == 204) {
        this.setCatalog(null);
        this.setCatalogMeta(null);
        this.setCatalogLinks(null);
        return;
      }

      const responseObject = await response.json();

      if (response.ok) {
        const catalogItems: ArticleModel[] = (responseObject.data ?? []).map(
          (item: any) =>
            ArticleModel.fromCatalogResponse({
              ...item,
            }),
        );
        this.setCatalog(catalogItems);
        this.setCatalogMeta(responseObject.meta);
        this.setCatalogLinks(responseObject.links);
        return;
      }
    },

    setCatalogMeta(catalogMeta: any) {
      this.catalogMeta = catalogMeta || [];
    },
    setCatalogLinks(catalogLinks: any) {
      this.catalogLinks = catalogLinks || [];
    },
    setCatalog(catalog: ArticleModel[] | null) {
      this.catalog = catalog || [];
    },

    async updateArticle(article: ArticleModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${article.id}`,
        {
          method: "PATCH",
          body: JSON.stringify({
            articleCode: article.articleCode,
            title: article.title,
            basePrice: article.basePrice ?? article.price,
            description: article.description,
            isAvailable: article.isAvailable,
          }),
        },
      );
      if (response.ok) {
        try {
          const payload = await response.clone().json();
          article.articleCode =
            payload.articleCode ?? payload.code ?? article.articleCode;
          article.title = payload.title ?? article.title;
          article.basePrice =
            payload.basePrice ?? payload.price ?? article.basePrice;
          article.price = payload.price ?? payload.basePrice ?? article.price;
          article.discount = payload.discount ?? article.discount ?? null;
          article.description = payload.description ?? article.description;
          article.createdAt = payload.createdAt ?? article.createdAt;
          article.updatedAt = payload.updatedAt ?? article.updatedAt;
          article.isAvailable = payload.isAvailable ?? article.isAvailable;

          const mergedCatalogItem = ArticleModel.fromCatalogResponse({
            ...payload,
            thumbnailImage: article.thumbnailImage ?? payload.thumbnailImage,
          });

          if (this.catalog) {
            const index = this.catalog.findIndex((p) => p.id == article.id);
            if (index > -1) {
              this.catalog[index] = mergedCatalogItem;
            }
          }
        } catch (error) {
          console.error("Failed to update catalog item cache", error);
        }
      }
      return response;
    },
    async deleteArticle(article: ArticleModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${article.id}`,
        {
          method: "DELETE",
        },
      );
      if (response.ok) {
        if (this.catalog) {
          const index = this.catalog.findIndex((p) => p.id == article.id);
          if (index > -1) {
            this.catalog.splice(index, 1);
          }
        }
      }
      return response;
    },
    async createArticle(article: ArticleModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles`,
        {
          method: "POST",
          body: JSON.stringify({
            articleCode: article.articleCode,
            title: article.title,
            basePrice: article.basePrice ?? article.price,
            description: article.description,
            isAvailable: article.isAvailable,
          }),
        },
      );
      if (response.ok) {
        try {
          const payload = await response.clone().json();
          if (this.catalog) {
            this.catalog.push(
              ArticleModel.fromCatalogResponse({
                ...payload,
                thumbnailImage:
                  payload.thumbnailImage ?? article.thumbnailImage ?? null,
              }),
            );
          }
          article.id = payload.id ?? article.id;
          article.articleCode =
            payload.articleCode ?? payload.code ?? article.articleCode;
          article.basePrice =
            payload.basePrice ?? payload.price ?? article.basePrice;
          article.price = payload.price ?? payload.basePrice ?? article.price;
          article.discount = payload.discount ?? null;
          article.createdAt = payload.createdAt ?? article.createdAt;
          article.updatedAt = payload.updatedAt ?? article.updatedAt;
          article.isAvailable = payload.isAvailable ?? article.isAvailable;
        } catch (error) {
          console.error("Failed to parse article create response", error);
        }
      }
      return response;
    },

    async createArticleDetail(
      articleId: string,
      payload: {
        detailSlug: string;
        textValue?: string;
        numericValue?: number;
      },
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/details`,
        {
          method: "POST",
          body: JSON.stringify({
            detailSlug: payload.detailSlug,
            textValue: payload.textValue,
            numericValue: payload.numericValue,
          }),
        },
      );
    },
    async deleteArticleDetail(
      articleId: string,
      detailSlug: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/details/${encodeURIComponent(detailSlug)}`,
        {
          method: "DELETE",
        },
      );
    },
    async updateArticleDetail(
      articleId: string,
      detailSlug: string,
      payload: { textValue?: string; numericValue?: number },
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/details/${encodeURIComponent(detailSlug)}`,
        {
          method: "PATCH",
          body: JSON.stringify({
            textValue: payload.textValue,
            numericValue: payload.numericValue,
          }),
        },
      );
    },

    async queryDetails(
      searchTerm: string,
      limit: number = 10,
    ): Promise<DetailOption[]> {
      const config = useRuntimeConfig();
      const params = new URLSearchParams();
      if (searchTerm?.trim()) {
        params.set("titleLike", searchTerm.trim());
      }
      if (limit) {
        params.set("limit", limit.toString());
      }
      const query = params.toString();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/details${
          query ? `?${query}` : ""
        }`,
        {
          method: "GET",
        },
      );
      return (await response.json()) ?? [];
    },

    async createArticleImage(
      articleId: string,
      orderIndex: number,
      file: File,
      imageAlt: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const formData = new FormData();
      formData.append("OrderIndex", orderIndex.toString());
      formData.append("File", file);
      if (imageAlt) {
        formData.append("ImageAlt", imageAlt);
      }

      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/images`,
        {
          method: "POST",
          body: formData,
        },
      );
      return response;
    },
    async updateArticleImage(
      articleId: string,
      orderIndex: number,
      payload: { orderIndex?: number; imageAlt?: string },
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const body: Record<string, unknown> = {};
      if (typeof payload.orderIndex === "number") {
        body.orderIndex = payload.orderIndex;
      }
      if (payload.imageAlt !== undefined) {
        body.imageAlt = payload.imageAlt;
      }
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/images/${orderIndex}`,
        {
          method: "PATCH",
          body: JSON.stringify(body),
        },
      );
      return response;
    },
    async deleteArticleImage(
      articleId: string,
      orderIndex: number,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/admin/articles/${articleId}/images/${orderIndex}`,
        {
          method: "DELETE",
        },
      );
      return response;
    },
  },
});
