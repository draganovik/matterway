function pad2(value: number) {
  return String(value).padStart(2, "0")
}

// Format: yyyyMMddHHmmssS, then encode decimal timestamp to hexadecimal.
export function createOrderReferenceId(date = new Date()) {
  const decimalTimestamp =
    `${date.getFullYear()}` +
    `${pad2(date.getMonth() + 1)}` +
    `${pad2(date.getDate())}` +
    `${pad2(date.getHours())}` +
    `${pad2(date.getMinutes())}` +
    `${pad2(date.getSeconds())}` +
    `${Math.floor(date.getMilliseconds() / 100)}`

  return BigInt(decimalTimestamp).toString(16)
}
