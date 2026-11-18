import { Navigate, Route, Routes } from 'react-router'
import Layout from './components/Layout'
import MachineDetailPage from './pages/MachineDetailPage'
import MachinesPage from './pages/MachinesPage'
import NotFoundPage from './pages/NotFoundPage'

/* Sve rute aplikacije, svaka stranica se prikazuje unutar zajedničkog Layouta. */
function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Navigate to="/machines" replace />} />
        <Route path="machines" element={<MachinesPage />} />
        <Route path="machines/:id" element={<MachineDetailPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}

export default App