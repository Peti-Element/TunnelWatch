type StatusBadgeProps = {
  online: boolean
}

function StatusBadge({ online }: StatusBadgeProps) {
  return <span className={`status ${online ? 'status-online' : 'status-offline'}`}>{online ? 'Online' : 'Offline'}</span>
}

export default StatusBadge