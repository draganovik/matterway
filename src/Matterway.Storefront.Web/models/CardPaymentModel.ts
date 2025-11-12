export default class CardPaymentModel {
  cardNumber: string;
  expMonth: number;
  expYear: number;
  cvc: string;
  amount: number;

  constructor(
    cardNumber: string = "",
    expMonth: number = new Date().getMonth() + 1,
    expYear: number = new Date().getFullYear(),
    cvc: string = "",
    amount: number = 0,
  ) {
    this.cardNumber = cardNumber;
    this.expMonth = expMonth;
    this.expYear = expYear;
    this.cvc = cvc;
    this.amount = amount;
  }
}
