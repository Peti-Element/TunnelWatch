import { useCallback } from 'react'
import { Link, useParams } from 'react-router'
import { getLatestSignals, NotFoundError } from '../api/gateways'
import StateMessage from '../components/StateMessage'
import { formatDateTime, formatValue } from '../format'
import { usePolling } from '../hooks/usePolling'

const REFRESH_MS = 10_000

function MachineDetailPage() {
  const { id = '' } = useParams<{ id: string }>()

  const load = useCallback((signal: AbortSignal) => getLatestSignals(id, signal), [id])
  const { data: signals, error, loading, lastUpdated } = usePolling(load, REFRESH_MS)

  const notFound = error instanceof NotFoundError

  return (
    <section>
      <p>
        <Link to="/machines">← Svi strojevi</Link>
      </p>
      <h1>Stroj {id}</h1>
      <p className="subtitle">
        Zadnje vrijednosti signala, osvježava se svakih 10 sekundi.
        {lastUpdated && <> Zadnje osvježavanje: {lastUpdated.toLocaleTimeString()}.</>}
      </p>

      {loading && <StateMessage>Učitavanje signala…</StateMessage>}

      {!loading && notFound && <StateMessage>Nema podataka za uređaj {id}.</StateMessage>}

      {!loading && error !== undefined && !notFound && signals === undefined && (
        <StateMessage kind="error">Backend nije dostupan. Provjeri je li pokrenut; pokušavam ponovno svakih 10 sekundi.</StateMessage>
      )}

      {!loading && error !== undefined && !notFound && signals !== undefined && (
        <StateMessage kind="warning">Osvježavanje nije uspjelo, prikazani su zadnji dohvaćeni podaci.</StateMessage>
      )}

      {signals !== undefined && signals.length > 0 && !notFound && (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Signal</th>
                <th className="numeric">Vrijednost</th>
                <th>Vrijeme očitanja</th>
              </tr>
            </thead>
            <tbody>
              {signals.map((signal) => (
                <tr key={signal.name}>
                  <td className="signal-name">{signal.name}</td>
                  <td className="numeric">{formatValue(signal.value, signal.unit)}</td>
                  <td>{formatDateTime(signal.timestamp, true, lastUpdated)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default MachineDetailPage