import { Link, useParams } from 'react-router'

/* Stranica /machines/:id: detalji jednog stroja. ID dolazi iz URL-a, npr. /machines/tdce-01. */
function MachineDetailPage() {
  const { id } = useParams<{ id: string }>()

  return (
    <section>
      <h1>Stroj {id}</h1>
      <p>Zadnje vrijednosti signala bit će prikazane ovdje.</p>
      <p><Link to="/machines">Natrag na popis</Link></p>
    </section>
  )
}

export default MachineDetailPage