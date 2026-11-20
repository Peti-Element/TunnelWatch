export const ONLINE_THRESHOLD_MS = 2 * 60 * 1000

export function isOnline(lastMessageAt: string, now: Date | undefined = new Date()): boolean {
  return now.getTime() - new Date(lastMessageAt).getTime() <= ONLINE_THRESHOLD_MS
}

function pad(value: number): string {
  return String(value).padStart(2, '0')
}

export function formatTime(date: Date): string {
  return `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`
}

export function formatDateTime(iso: string): string {
  const date = new Date(iso)
  return `${pad(date.getDate())}.${pad(date.getMonth() + 1)}.${date.getFullYear()} ${formatTime(date)}`
}

export function formatValue(value: number): string {
  return value.toLocaleString(undefined, { maximumFractionDigits: 3 })
}