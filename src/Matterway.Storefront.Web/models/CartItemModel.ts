import ArticleModel from "./ArticleModel";

export default class CartItemModel {
  articleId?: string;
  articleName?: string;
  unitPrice?: number;
  quantity: number = 1;

  totalPrice(): number {
    return (this.unitPrice ?? 0) * this.quantity;
  }

  constructor(article: ArticleModel) {
    this.articleId = article.id;
    this.articleName = article.title;
    this.unitPrice = article.price ?? article.basePrice ?? 0;
  }
}
