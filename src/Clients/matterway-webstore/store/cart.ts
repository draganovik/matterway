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
  },

  actions: {
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

      console.log(session.getTokenData);
      console.log("cart", this.cartItems);

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
      const session = useSessionStore();

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
