import { Fragment, useCallback } from 'react'
import { Link, useParams } from 'react-router'
import { getLatestSignals, NotFoundError } from '../api/gateways'
import StateMessage from '../components/StateMessage'
import StatusBadge from '../components/StatusBadge'
import { formatDateTime, formatValue, isOnline } from '../format'
import { usePolling } from '../hooks/usePolling'

const REFRESH_MS = 10_000

function breakableName(name: string) {
  return name.split('_').map((part, index) => (
    <Fragment key={index}>
      {index > 0 && <>_<wbr /></>}
      {part}
    </Fragment>
  ))
}

function MachineDetailPage() {
  const { id = '' } = useParams<{ id: string }>()

  const load = useCallback((signal: AbortSignal) => getLatestSignals(id, signal), [id])
  const { data: signals, error, loading, lastUpdated } = usePolling(load, REFRESH_MS)

  const notFound = error instanceof NotFoundError
  const hasSignals = signals !== undefined && signals.length > 0 && !notFound

  const newest = hasSignals ? signals.reduce((a, b) => (a.timestamp > b.timestamp ? a : b)).timestamp : undefined

  return (
    <section>
      <Link to="/machines" className="button">
        Svi strojevi
      </Link>

      <div className="page-header">
        <div>
          <h1>
            {id} {newest && <StatusBadge online={isOnline(newest, lastUpdated)} />}
          </h1>
          <p className="subtitle">Zadnje vrijednosti signala, osvježava se svakih 10 sekundi.</p>
        </div>
        {lastUpdated && (
          <p className={`refresh-info ${error === undefined ? 'refresh-ok' : 'refresh-failed'}`}>
            Osvježeno {lastUpdated.toLocaleTimeString()}
          </p>
        )}
      </div>

      {loading && <StateMessage>Učitavanje signala…</StateMessage>}

      {!loading && notFound && <StateMessage>Nema podataka za uređaj {id}.</StateMessage>}

      {!loading && error !== undefined && !notFound && signals === undefined && (
        <StateMessage kind="error">Backend nije dostupan. Provjeri je li pokrenut; pokušavam ponovno svakih 10 sekundi.</StateMessage>
      )}

      {!loading && error !== undefined && !notFound && signals !== undefined && (
        <StateMessage kind="warning">Osvježavanje nije uspjelo, prikazani su zadnji dohvaćeni podaci.</StateMessage>
      )}

      {hasSignals && (
        <div className="card table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Signal</th>
                <th className="numeric">Vrijednost</th>
                <th className="desktop-only">Vrijeme očitanja</th>
              </tr>
            </thead>
            <tbody>
              {signals.map((signal) => {
                const time = formatDateTime(signal.timestamp, true, lastUpdated)
                return (
                  <tr key={signal.name}>
                    <td>
                      <span className="signal-name">{breakableName(signal.name)}</span>
                      <span className="mobile-only muted">{time}</span>
                    </td>
                    <td className="numeric value">{formatValue(signal.value, signal.unit)}</td>
                    <td className="muted desktop-only">{time}</td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default MachineDetailPage