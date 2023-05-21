export function formatMoney(price: number, currency: string = "RSD"): string {
  return price.toLocaleString(getUserLocale(), {
    style: "currency",
    currency: currency,
  });
}

export function getUserLocale(): string {
  return "rs-RS";
  //return navigator.language || "rs-RS";
}
