import { NavLink, Outlet } from 'react-router'

/* Zajednički okvir svih stranica: zaglavlje s navigacijom, a ispod njega trenutna stranica. */
function Layout() {
  return (
    <>
      <header className="app-header">
        <span className="app-title">TunnelWatch</span>
        <nav>
          <NavLink to="/machines">Strojevi</NavLink>
        </nav>
      </header>
      <main className="app-main">
        <Outlet />
      </main>
    </>
  )
}

export default Layout