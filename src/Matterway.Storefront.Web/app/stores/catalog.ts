// stores/catalog.ts

import { defineStore } from "pinia";
import ProductModel from "#models/ProductModel";

interface CatalogState {
  catalog: ProductModel[] | null;
  catalogMeta: any;
  catalogLinks: any;
}

export interface DetailOption {
  slug: string;
  title?: string;
}

export interface SpecificationOption {
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
    getCatalogData(): ProductModel[] | null {
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
    async fetchProductById(id: string): Promise<ProductModel | null> {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${id}`,
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
          return ProductModel.fromDetailResponse(responseObject);
        } catch (error) {
          console.error("Failed to parse product response", error);
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
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products?${params.toString()}`,
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
        const catalogItems: ProductModel[] = (responseObject.data ?? []).map(
          (item: any) =>
            ProductModel.fromCatalogResponse({
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
    setCatalog(catalog: ProductModel[] | null) {
      this.catalog = catalog || [];
    },

    async updateProduct(product: ProductModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${product.id}`,
        {
          method: "PATCH",
          body: JSON.stringify({
            productCode: product.productCode,
            title: product.title,
            price: product.price ?? product.basePrice,
            description: product.description,
            isAvailable: product.isAvailable,
          }),
        },
      );
      if (response.ok) {
        try {
          const payload = await response.clone().json();
          product.productCode =
            payload.productCode ?? payload.code ?? product.productCode;
          product.title = payload.title ?? product.title;
          product.basePrice =
            payload.basePrice ?? payload.price ?? product.basePrice;
          product.price = payload.price ?? payload.basePrice ?? product.price;
          product.discount = payload.discount ?? product.discount ?? null;
          product.description = payload.description ?? product.description;
          product.createdAt = payload.createdAt ?? product.createdAt;
          product.updatedAt = payload.updatedAt ?? product.updatedAt;
          product.isAvailable = payload.isAvailable ?? product.isAvailable;

          const mergedCatalogItem = ProductModel.fromCatalogResponse({
            ...payload,
            thumbnailImage: product.thumbnailImage ?? payload.thumbnailImage,
          });

          if (this.catalog) {
            const index = this.catalog.findIndex((p) => p.id == product.id);
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
    async deleteProduct(product: ProductModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${product.id}`,
        {
          method: "DELETE",
        },
      );
      if (response.ok) {
        if (this.catalog) {
          const index = this.catalog.findIndex((p) => p.id == product.id);
          if (index > -1) {
            this.catalog.splice(index, 1);
          }
        }
      }
      return response;
    },
    async createProduct(product: ProductModel): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products`,
        {
          method: "POST",
          body: JSON.stringify({
            productCode: product.productCode,
            title: product.title,
            price: product.price ?? product.basePrice,
            description: product.description,
            isAvailable: product.isAvailable,
          }),
        },
      );
      if (response.ok) {
        try {
          const payload = await response.clone().json();
          if (this.catalog) {
            this.catalog.push(
              ProductModel.fromCatalogResponse({
                ...payload,
                thumbnailImage:
                  payload.thumbnailImage ?? product.thumbnailImage ?? null,
              }),
            );
          }
          product.id = payload.id ?? product.id;
          product.productCode =
            payload.productCode ?? payload.code ?? product.productCode;
          product.basePrice =
            payload.basePrice ?? payload.price ?? product.basePrice;
          product.price = payload.price ?? payload.basePrice ?? product.price;
          product.discount = payload.discount ?? null;
          product.createdAt = payload.createdAt ?? product.createdAt;
          product.updatedAt = payload.updatedAt ?? product.updatedAt;
          product.isAvailable = payload.isAvailable ?? product.isAvailable;
        } catch (error) {
          console.error("Failed to parse product create response", error);
        }
      }
      return response;
    },

    async createProductDetail(
      productId: string,
      detailSlug: string,
      value: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Details`,
        {
          method: "POST",
          body: JSON.stringify({
            detailSlug: detailSlug,
            value,
          }),
        },
      );
    },
    async deleteProductDetail(
      productId: string,
      detailSlug: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Details/${encodeURIComponent(detailSlug)}`,
        {
          method: "DELETE",
        },
      );
    },
    async updateProductDetail(
      productId: string,
      detailSlug: string,
      value: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Details/${encodeURIComponent(detailSlug)}`,
        {
          method: "PATCH",
          body: JSON.stringify({ value }),
        },
      );
    },

    async createProductSpecification(
      productId: string,
      specificationSlug: string,
      value: number,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Specifications`,
        {
          method: "POST",
          body: JSON.stringify({
            specificationSlug,
            value,
          }),
        },
      );
    },
    async deleteProductSpecification(
      productId: string,
      specificationSlug: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Specifications/${encodeURIComponent(specificationSlug)}`,
        {
          method: "DELETE",
        },
      );
    },
    async updateProductSpecification(
      productId: string,
      specificationSlug: string,
      value: number,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      return await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Specifications/${encodeURIComponent(specificationSlug)}`,
        {
          method: "PATCH",
          body: JSON.stringify({ value }),
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
        `${config.public.catalogApiBaseUrl}/api/v1.0/Details${
          query ? `?${query}` : ""
        }`,
        {
          method: "GET",
        },
      );
      return (await response.json()) ?? [];
    },

    async querySpecifications(
      searchTerm: string,
      limit: number = 10,
    ): Promise<SpecificationOption[]> {
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
        `${config.public.catalogApiBaseUrl}/api/v1.0/Specifications${
          query ? `?${query}` : ""
        }`,
        { method: "GET" },
      );
      return (await response.json()) ?? [];
    },

    async createProductImage(
      productId: string,
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
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Images`,
        {
          method: "POST",
          body: formData,
        },
      );
      return response;
    },
    async updateProductImage(
      productId: string,
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
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Images/${orderIndex}`,
        {
          method: "PATCH",
          body: JSON.stringify(body),
        },
      );
      return response;
    },
    async deleteProductImage(
      productId: string,
      orderIndex: number,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products/${productId}/Images/${orderIndex}`,
        {
          method: "DELETE",
        },
      );
      return response;
    },
  },
});
