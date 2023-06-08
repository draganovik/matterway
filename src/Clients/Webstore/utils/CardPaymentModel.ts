export default class CardPaymentModel {
  cardNumber: string;
  expMonth: number;
  expYear: number;
  cvc: string;
  amount: number;
  constructor(
    cardNumber: string,
    expMonth: number,
    expYear: number,
    cvc: string,
    amount: number,
  ) {
    this.cardNumber = cardNumber;
    this.expMonth = expMonth;
    this.expYear = expYear;
    this.cvc = cvc;
    this.amount = amount;
  }
}
