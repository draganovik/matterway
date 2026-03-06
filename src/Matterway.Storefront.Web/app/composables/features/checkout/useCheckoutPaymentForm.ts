import type { CheckoutPaymentForm, ParsedCardExpiry } from "~/types/checkout"

function isLuhnValid(cardDigits: string) {
  let sum = 0
  let shouldDouble = false

  for (let index = cardDigits.length - 1; index >= 0; index -= 1) {
    let digit = Number(cardDigits[index])
    if (!Number.isFinite(digit)) {
      return false
    }

    if (shouldDouble) {
      digit *= 2
      if (digit > 9) {
        digit -= 9
      }
    }

    sum += digit
    shouldDouble = !shouldDouble
  }

  return sum % 10 === 0
}

function formatCardNumber(value: string) {
  const digits = value.replace(/\D/g, "").slice(0, 19)
  return digits.replace(/(.{4})/g, "$1 ").trim()
}

export function useCheckoutPaymentForm() {
  const payment = reactive<CheckoutPaymentForm>({
    cardNumber: "",
    expMonth: "",
    expYear: "",
    cvc: "",
  })

  const currentYear = new Date().getFullYear()

  function getCardDigits() {
    return payment.cardNumber.replace(/\D/g, "")
  }

  function getParsedExpiry(): ParsedCardExpiry | null {
    const monthRaw = String(payment.expMonth ?? "").trim()
    const yearRaw = String(payment.expYear ?? "").trim()

    if (!/^\d{2}$/.test(monthRaw)) return null
    if (!/^\d{4}$/.test(yearRaw)) return null

    const month = Number(monthRaw)
    const year = Number(yearRaw)
    if (!Number.isInteger(month) || !Number.isInteger(year)) return null
    if (month < 1 || month > 12) return null
    if (year < 2000) return null

    const now = new Date()
    const nowMonth = now.getMonth() + 1
    const nowYear = now.getFullYear()
    if (year < nowYear) return null
    if (year === nowYear && month < nowMonth) return null

    return { month, year }
  }

  function getValidationError() {
    const cardDigits = getCardDigits()
    if (!/^\d{13,19}$/.test(cardDigits) || !isLuhnValid(cardDigits)) {
      return "Unesite ispravan broj kartice."
    }

    if (!getParsedExpiry()) {
      return "Unesite ispravan datum isteka kartice."
    }

    if (!/^\d{3,4}$/.test(payment.cvc)) {
      return "Unesite ispravan CVC kod."
    }

    return null
  }

  watch(
    () => payment.cardNumber,
    (value) => {
      const formatted = formatCardNumber(value)
      if (formatted !== value) {
        payment.cardNumber = formatted
      }
    },
  )

  watch(
    () => payment.cvc,
    (value) => {
      const normalized = value.replace(/\D/g, "").slice(0, 4)
      if (normalized !== value) {
        payment.cvc = normalized
      }
    },
  )

  watch(
    () => payment.expMonth,
    (value) => {
      let normalized = String(value ?? "")
        .replace(/\D/g, "")
        .slice(0, 2)

      if (normalized.length === 2 && Number(normalized) > 12) {
        normalized = "12"
      }

      if (normalized !== value) {
        payment.expMonth = normalized
      }
    },
  )

  watch(
    () => payment.expYear,
    (value) => {
      const normalized = String(value ?? "")
        .replace(/\D/g, "")
        .slice(0, 4)
      if (normalized !== value) {
        payment.expYear = normalized
      }
    },
  )

  return {
    payment,
    currentYear,
    getCardDigits,
    getParsedExpiry,
    getValidationError,
  }
}
