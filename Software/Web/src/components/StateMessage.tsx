import type { ReactNode } from 'react'

type StateMessageProps = {
  kind?: 'info' | 'error' | 'warning'
  children: ReactNode
}

function StateMessage({ kind = 'info', children }: StateMessageProps) {
  return (
    <p className={`state state-${kind}`} role={kind === 'info' ? 'status' : 'alert'}>
      {children}
    </p>
  )
}

export default StateMessage