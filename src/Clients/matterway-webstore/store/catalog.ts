// store/catalog.ts

import { defineStore } from "pinia";
import ProductModel from "~/utils/ProductModel";

interface CatalogState {
  catalog: ProductModel[] | null;
  catalogMeta: any;
}

export const useCatalogStore = defineStore("catalog", {
  state: (): CatalogState => ({
    catalog: null,
    catalogMeta: null,
  }),

  getters: {
    getCatalogData(): ProductModel[] | null {
      return this.catalog;
    },
  },

  actions: {
    async fetchCatalog(page: number = 1, pageSize: number = 10) {
      const config = useRuntimeConfig();
      const response = await fetch(
        `${config.public.catalog_api_base_url}/api/Products?page=${page}&pageSize=${pageSize}`,
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
        this.setCatalog(responseObject.data);
        this.setCatalogMeta(responseObject.meta);
      }
    },
    setCatalogMeta(catalogMeta: any) {
      this.catalogMeta = catalogMeta;
    },
    setCatalog(catalog: ProductModel[] | null) {
      this.catalog = catalog;
    },
  },
});
