export default class ProductModel {
  id!: string;
  productCode!: string;
  title!: string;
  price!: number;
  description!: string;
  productDetails!: ProductDetails[];
  productImages!: ProductImages[];
  createdAt!: string;
  updatedAt!: string;
  isAvailable!: boolean;
}

export class ProductDetails {
  id!: string;
  type!: string;
  title!: string;
  value!: string;
  unit!: string;
}

export class ProductImages {
  imageUrl!: string;
  imageAlt!: string;
  isMain!: boolean;
}
