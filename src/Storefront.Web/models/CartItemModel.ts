import ProductModel from "./ProductModel";

export default class CartItemModel {
  productId?: string;
  productName?: string;
  unitPrice?: number;
  quantity: number = 1;

  totalPrice(): number {
    return this.unitPrice || 0 * this.quantity;
  }

  constructor(product: ProductModel) {
    this.productId = product.id;
    this.productName = product.title;
    this.unitPrice = product.price;
  }
}
