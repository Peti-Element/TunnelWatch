import { useState } from 'react'
import { Link } from 'react-router'
import { getGateways } from '../api/gateways'
import StateMessage from '../components/StateMessage'
import { formatDateTime, isOnline } from '../format'
import { usePolling } from '../hooks/usePolling'

type StatusFilter = 'all' | 'online' | 'offline'

function MachinesPage() {
  const { data: gateways, error, loading, lastUpdated } = usePolling(getGateways, 10_000)
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<StatusFilter>('all')

  const now = lastUpdated
  const visible = (gateways ?? []).filter((gateway) => {
    const online = isOnline(gateway.lastMessageAt, now)
    const matchesSearch = gateway.id.toLowerCase().includes(search.trim().toLowerCase())
    const matchesStatus = status === 'all' || (status === 'online') === online
    return matchesSearch && matchesStatus
  })

  return (
    <section>
      <h1>Strojevi</h1>
      <p className="subtitle">Pregled svih strojeva i vremena njihove zadnje poruke.</p>

      <div className="toolbar">
        <input
          type="search"
          placeholder="Pretraži strojeve…"
          aria-label="Pretraži strojeve"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        <select
          aria-label="Filtriraj po statusu"
          value={status}
          onChange={(event) => setStatus(event.target.value as StatusFilter)}
        >
          <option value="all">Svi statusi</option>
          <option value="online">Online</option>
          <option value="offline">Offline</option>
        </select>
      </div>

      {loading && <StateMessage>Učitavanje strojeva…</StateMessage>}

      {!loading && error !== undefined && gateways === undefined && (
        <StateMessage kind="error">Backend nije dostupan. Provjeri je li pokrenut; pokušavam ponovno svakih 10 sekundi.</StateMessage>
      )}

      {!loading && error !== undefined && gateways !== undefined && (
        <StateMessage kind="warning">Osvježavanje nije uspjelo, prikazani su zadnji dohvaćeni podaci.</StateMessage>
      )}

      {gateways !== undefined && gateways.length === 0 && (
        <StateMessage>Još nijedan stroj nije poslao podatke.</StateMessage>
      )}

      {gateways !== undefined && gateways.length > 0 && visible.length === 0 && (
        <StateMessage>Nijedan stroj ne odgovara pretrazi.</StateMessage>
      )}

      {visible.length > 0 && (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Stroj</th>
                <th>Status</th>
                <th>Zadnja poruka</th>
              </tr>
            </thead>
            <tbody>
              {visible.map((gateway) => {
                const online = isOnline(gateway.lastMessageAt, now)
                return (
                  <tr key={gateway.id}>
                    <td>
                      <Link className="machine-link" to={`/machines/${encodeURIComponent(gateway.id)}`}>
                        {gateway.id}
                      </Link>
                    </td>
                    <td>
                      <span className={`status status-${online ? 'online' : 'offline'}`}>
                        {online ? 'Online' : 'Offline'}
                      </span>
                    </td>
                    <td>{formatDateTime(gateway.lastMessageAt, false, now)}</td>
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

export default MachinesPage