import { Link } from 'react-router'

/* Prikazuje se za svaku adresu koja ne odgovara nijednoj ruti. */
function NotFoundPage() {
  return (
    <section>
      <h1>Stranica ne postoji</h1>
      <p><Link to="/machines">Idi na popis strojeva</Link></p>
    </section>
  )
}

export default NotFoundPage