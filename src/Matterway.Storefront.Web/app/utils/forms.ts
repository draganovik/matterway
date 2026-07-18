export function submitFormOnEnter(event: KeyboardEvent) {
  if (event.isComposing) return

  event.preventDefault()
  const input = event.currentTarget as HTMLInputElement
  input.form?.requestSubmit()
}
