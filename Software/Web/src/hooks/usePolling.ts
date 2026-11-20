import { useCallback, useEffect, useState } from 'react'

export type PollingState<T> = {
  data: T | undefined
  error: unknown
  loading: boolean
  lastUpdated: Date | undefined
}

const initialState = { data: undefined, error: undefined, loading: true, lastUpdated: undefined }

type Load<T> = (signal: AbortSignal) => Promise<T>

export function usePolling<T>(load: Load<T>, intervalMs: number): PollingState<T> & { retry: () => void } {
  const [state, setState] = useState<PollingState<T> & { source: Load<T> }>({ ...initialState, source: load })
  const [attempt, setAttempt] = useState(0)
  const retry = useCallback(() => setAttempt((value) => value + 1), [])

  useEffect(() => {
    const controller = new AbortController()
    let timer: number | undefined

    async function run() {
      try {
        const data = await load(controller.signal)
        if (controller.signal.aborted) {
          return
        }
        setState({ data, error: undefined, loading: false, lastUpdated: new Date(), source: load })
      } catch (error) {
        if (controller.signal.aborted) {
          return
        }
        setState((previous) =>
          previous.source === load
            ? { ...previous, error, loading: false }
            : { ...initialState, error, loading: false, source: load },
        )
      }

      timer = window.setTimeout(run, intervalMs)
    }

    void run()

    return () => {
      controller.abort()
      window.clearTimeout(timer)
    }
    }, [load, intervalMs, attempt])

  return { ...(state.source === load ? state : initialState), retry }
}