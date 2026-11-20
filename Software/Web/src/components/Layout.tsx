import { Link, NavLink, Outlet } from 'react-router'

function Layout() {
  return (
    <>
      <header className="app-header">
        <div className="app-header-inner">
          <Link to="/machines" className="brand">
            <span className="brand-mark" aria-hidden="true">TW</span>
            TunnelWatch
          </Link>
          <nav className="app-nav">
            <NavLink to="/machines">Strojevi</NavLink>
          </nav>
        </div>
      </header>
      <main className="app-main">
        <Outlet />
      </main>
    </>
  )
}

export default Layout