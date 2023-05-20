// store/cart.ts

import { defineStore } from "pinia";
import CartItemModel from "~/utils/CartItemModel";
import { useSessionStore } from "./session";
import ProductModel from "~/utils/ProductModel";

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
    isProductInCart(): (productId: string) => boolean {
      return (productId: string) =>
        this.cartItems?.some((item) => item.productId === productId);
    },
    countProductsInCart(): (productId: string) => number {
      return (productId: string) =>
        this.cartItems?.find((item) => item.productId === productId)
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
    async removeFromCart(product: ProductModel) {
      const config = useRuntimeConfig();
      const session = useSessionStore();
      const findItem = this.cartItems.find(
        (item) => item.productId === product.id,
      );
      if (findItem && findItem.quantity > 1) {
        findItem.quantity--;
        const response = await request(
          `${config.public.customers_api_base_url}/api/Customers/${session.getTokenData?.nameid}/CartItems/${findItem?.productId}`,
          {
            method: "PUT",
            body: JSON.stringify({ quantity: findItem.quantity }),
          },
        );
        if (response.ok) {
          console.log(await response.json());
        }
      } else {
        this.cartItems = this.cartItems.filter(
          (item) => item.productId !== product.id,
        );
        await request(
          `${config.public.customers_api_base_url}/api/Customers/${session.getTokenData?.nameid}/CartItems/${product.id}`,
          {
            method: "DELETE",
          },
        );
      }
    },
    async addToCart(product: ProductModel) {
      const config = useRuntimeConfig();
      const session = useSessionStore();

      // if product exist in cart set quantity to +1, else add product to cart
      if (this.cartItems?.some((item) => item.productId === product.id)) {
        this.cartItems.find((item) => item.productId === product.id)!
          .quantity++;
      } else {
        this.cartItems?.push(new CartItemModel(product));
      }

      const currentItem = this.cartItems?.find(
        (item) => item.productId === product.id,
      );

      const response = await request(
        `${config.public.customers_api_base_url}/api/Customers/${session.getTokenData?.nameid}/CartItems/${currentItem?.productId}`,
        {
          method: "PUT",
          body: JSON.stringify({ quantity: currentItem?.quantity }),
        },
      );
      if (response.ok) {
        console.log(await response.json());
      }
    },

    async fetchCartItems(page: number = 1, pageSize: number = 10) {
      const config = useRuntimeConfig();

      const response = await request(
        `${config.public.customers_api_base_url}/api/Customers/CartItems?page=${page}&pageSize=${pageSize}`,
        {
          method: "GET",
        },
      );
      if (response.ok) {
        let responseItems = await response.json();
        this.cartItems = responseItems.data;
      }
    },
  },
});
