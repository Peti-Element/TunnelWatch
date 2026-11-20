import type { ReactNode } from 'react'

type StateMessageProps = {
  kind?: 'info' | 'error' | 'warning'
  children: ReactNode
  onRetry?: () => void
}

function StateMessage({ kind = 'info', children, onRetry }: StateMessageProps) {
  return (
    <div className={`state state-${kind}`} role={kind === 'info' ? 'status' : 'alert'}>
      <span>{children}</span>
      {onRetry && (
        <button type="button" className="button" onClick={onRetry}>
          Pokušaj ponovno
        </button>
      )}
    </div>
  )
}

export default StateMessage