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
        `${config.public.catalog_api_base_url}/api/Products/${id}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            accept: "application/json",
          },
        },
      );
      const responseObject = await response.json();
      if (response.ok) {
        return responseObject;
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
        `${config.public.catalog_api_base_url}/api/Products?page=${page}&pageSize=${pageSize}${advancedQuery}`,
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
        this.setCatalog(responseObject.data);
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
  },
});
