// store/catalog.ts

import { defineStore } from "pinia";
import ProductModel from "~/utils/ProductModel";

interface CatalogState {
  catalog: ProductModel[] | null;
  catalogMeta: any;
  catalogLinks: any;
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
          console.error("Failed to parse product detail response", error);
        }
      }
      return null;
    },
    async fetchCatalog(
      page: number = 1,
      pageSize: number = 10,
      titleLike: string = "",
      priceMin: number = 0,
      priceMax: number = 0,
    ) {
      let advancedQuery = "";
      if (titleLike != "") {
        advancedQuery = `&TitleLike=${titleLike}`;
      }
      if (priceMin > 0) {
        advancedQuery = `${advancedQuery}&PriceMin=${priceMin}`;
      }
      if (priceMax > 0) {
        advancedQuery = `${advancedQuery}&PriceMax=${priceMax}`;
      }
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.catalogApiBaseUrl}/api/v1.0/Products?page=${page}&pageSize=${pageSize}${advancedQuery}`,
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
            price: product.price,
            description: product.description,
            isAvailable: product.isAvailable,
          }),
        },
      );
      if (response.ok) {
        try {
          const payload = await response.clone().json();
          product.productCode = payload.productCode ?? product.productCode;
          product.title = payload.title ?? product.title;
          product.price = payload.price ?? product.price;
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
      console.log(response);
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
            price: product.price,
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
          product.createdAt = payload.createdAt ?? product.createdAt;
          product.updatedAt = payload.updatedAt ?? product.updatedAt;
          product.isAvailable = payload.isAvailable ?? product.isAvailable;
        } catch (error) {
          console.error("Failed to parse product create response", error);
        }
      }
      return response;
    },

    async createProductSpec(
      productId: string,
      title: string,
      value: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/ProductDetails`,
        {
          method: "POST",
          body: JSON.stringify({
            productId: productId,
            type: "Specification",
            title: title,
            value: value,
          }),
        },
      );
      console.log(response);
      return response;
    },
    async deleteProductSpec(specId: string): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/ProductDetails/${specId}`,
        {
          method: "DELETE",
        },
      );
      console.log(response);
      return response;
    },
    async updateProductSpec(
      specId: string,
      productId: string,
      title: string,
      value: string,
      type: string | number = "Specification",
      unit?: string | null,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const payload: Record<string, unknown> = {
        productId: productId,
        type: type,
        title: title,
        value: value,
      };
      if (unit !== undefined) {
        payload.unit = unit;
      }
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/ProductDetails/${specId}`,
        {
          method: "PATCH",
          body: JSON.stringify(payload),
        },
      );
      console.log(response);
      return response;
    },

    async createProductImage(
      productId: string,
      imageId: number,
      imageUrl: string,
      imageAlt: string,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/ProductImages`,
        {
          method: "POST",
          body: JSON.stringify({
            id: imageId,
            productId: productId,
            imageUrl: imageUrl,
            imageAlt: imageAlt,
            isMain: false,
          }),
        },
      );
      console.log(response);
      return response;
    },
    async deleteProductImage(
      productId: string,
      imageId: number,
    ): Promise<Response> {
      const config = useRuntimeConfig();
      const response = await request(
        `${config.public.catalogApiBaseUrl}/api/v1.0/ProductImages/${productId}/${imageId}`,
        {
          method: "DELETE",
        },
      );
      console.log(response);
      return response;
    },
  },
});
