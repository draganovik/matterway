export function createReferenceId(): string {
  const segment = () => Math.floor(1000 + Math.random() * 9000).toString()
  return `${segment()}-${segment()}-${segment()}-${segment()}`
}
