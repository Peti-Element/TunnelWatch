import { Link } from 'react-router'

/* Stranica /machines: popis strojeva. */
function MachinesPage() {
  return (
    <section>
      <h1>Strojevi</h1>
      <p>Popis strojeva bit će prikazan ovdje.</p>
      <p>Primjer detalja: <Link to="/machines/tdce-01">tdce-01</Link></p>
    </section>
  )
}

export default MachinesPage