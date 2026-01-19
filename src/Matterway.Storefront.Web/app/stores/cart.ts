// stores/cart.ts

import { defineStore } from "pinia";
import CartItemModel from "#models/CartItemModel";
import { useSessionStore } from "@stores/session";
import ArticleModel from "#models/ArticleModel";

interface CartState {
  cartItems: CartItemModel[];
}

export const useCartStore = defineStore("cart", {
  persist: true,
  state: (): CartState => ({
    cartItems: [],
  }),

  getters: {
    getCartItems(): CartItemModel[] {
      return this.cartItems;
    },
    isArticleInCart(): (articleId: string) => boolean {
      return (articleId: string) =>
        this.cartItems?.some((item) => item.articleId === articleId);
    },
    countArticlesInCart(): (articleId: string) => number {
      return (articleId: string) =>
        this.cartItems?.find((item) => item.articleId === articleId)
          ?.quantity || 0;
    },
    getTotalPrice(): number {
      return this.cartItems
        ? this.cartItems.reduce(
            (total, item) => total + item.quantity * (item.unitPrice || 0),
            0,
          )
        : 0;
    },
    getTotalItemCount(): number {
      return this.cartItems
        ? this.cartItems.reduce((total, item) => total + item.quantity, 0)
        : 0;
    },
  },

  actions: {
    async removeFromCart(article: ArticleModel) {
      const config = useRuntimeConfig();
      const session = useSessionStore();
      const findItem = this.cartItems.find(
        (item) => item.articleId === article.id,
      );
      if (findItem && findItem.quantity > 1) {
        findItem.quantity--;
        await request(
          `${config.public.customersApiBaseUrl}/api/v1.0/Customers/${session.getTokenData?.sub}/CartItems/${findItem?.articleId}`,
          {
            method: "PUT",
            body: JSON.stringify({ quantity: findItem.quantity }),
          },
        );
      } else {
        this.cartItems = this.cartItems.filter(
          (item) => item.articleId !== article.id,
        );
        await request(
          `${config.public.customersApiBaseUrl}/api/v1.0/Customers/${session.getTokenData?.sub}/CartItems/${article.id}`,
          {
            method: "DELETE",
          },
        );
      }
    },
    async addToCart(article: ArticleModel) {
      const config = useRuntimeConfig();
      const session = useSessionStore();

      // if article exist in cart set quantity to +1, else add article to cart
      if (this.cartItems?.some((item) => item.articleId === article.id)) {
        this.cartItems.find((item) => item.articleId === article.id)!
          .quantity++;
      } else {
        this.cartItems?.push(new CartItemModel(article));
      }

      const currentItem = this.cartItems?.find(
        (item) => item.articleId === article.id,
      );

      await request(
        `${config.public.customersApiBaseUrl}/api/v1.0/Customers/${session.getTokenData?.sub}/CartItems/${currentItem?.articleId}`,
        {
          method: "PUT",
          body: JSON.stringify({ quantity: currentItem?.quantity }),
        },
      );
    },
    async clearCart() {
      const config = useRuntimeConfig();
      const session = useSessionStore();
      this.cartItems.forEach(async (item) => {
        await request(
          `${config.public.customersApiBaseUrl}/api/v1.0/Customers/${session.getTokenData?.sub}/CartItems/${item.articleId}`,
          {
            method: "DELETE",
          },
        );
      });
      this.cartItems = [];
    },

    async fetchCartItems(page: number = 1, pageSize: number = 10) {
      const config = useRuntimeConfig();
      try {
        const response = await request(
          `${config.public.customersApiBaseUrl}/api/v1.0/Customers/CartItems?page=${page}&pageSize=${pageSize}`,
          {
            method: "GET",
          },
        );
        if (response.status === 204) {
          this.cartItems = [];
          return;
        }
        if (response.ok) {
          let responseItems = await response.json();
          this.cartItems = responseItems.data;
          return;
        }
      } catch (error) {
        this.cartItems = [];
        return;
      }
    },
  },
});
