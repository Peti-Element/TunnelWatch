export const ONLINE_THRESHOLD_MS = 2 * 60 * 1000

export function isOnline(lastMessageAt: string, now: Date | undefined = new Date()): boolean {
  return now.getTime() - new Date(lastMessageAt).getTime() <= ONLINE_THRESHOLD_MS
}

function isSameDay(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}

export function formatDateTime(iso: string, withSeconds = false, now: Date | undefined = new Date()): string {
  const date = new Date(iso)
  const time = date.toLocaleTimeString(undefined, {
    hour: '2-digit',
    minute: '2-digit',
    second: withSeconds ? '2-digit' : undefined,
  })

  const yesterday = new Date(now)
  yesterday.setDate(now.getDate() - 1)

  if (isSameDay(date, now)) {
    return `Danas, ${time}`
  }
  if (isSameDay(date, yesterday)) {
    return `Jučer, ${time}`
  }
  return `${date.toLocaleDateString()} ${time}`
}

export function formatValue(value: number, unit: string | null): string {
  const number = value.toLocaleString(undefined, { maximumFractionDigits: 3 })
  return unit ? `${number} ${unit}` : number
}