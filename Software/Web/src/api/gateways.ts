/* Uređaj iz GET /api/gateways. Vrijeme je ISO string u UTC-u, npr. "2026-11-07T04:26:21.662+00:00". */
export type Gateway = {
  id: string
  lastMessageAt: string
}

/* Zadnja vrijednost signala iz GET /api/gateways/{id}/latest.*/
export type LatestSignal = {
  name: string
  value: number
  unit: string | null
  timestamp: string
}

export class NotFoundError extends Error {}
export class BackendUnavailableError extends Error {}

async function getJson<T>(url: string, signal?: AbortSignal): Promise<T> {
  let response: Response
  try {
    response = await fetch(url, { signal, headers: { Accept: 'application/json' } })
  } catch (error) {
    if (signal?.aborted) {
      throw error
    }
    throw new BackendUnavailableError('Backend nije dostupan.')
  }

  if (response.status === 404) {
    throw new NotFoundError('Nema podataka.')
  }
  if (!response.ok) {
    throw new BackendUnavailableError(`Backend je vratio grešku ${response.status}.`)
  }

  return (await response.json()) as T
}

export function getGateways(signal?: AbortSignal): Promise<Gateway[]> {
  return getJson<Gateway[]>('/api/gateways', signal)
}

export function getLatestSignals(gatewayId: string, signal?: AbortSignal): Promise<LatestSignal[]> {
  return getJson<LatestSignal[]>(`/api/gateways/${encodeURIComponent(gatewayId)}/latest`, signal)
}