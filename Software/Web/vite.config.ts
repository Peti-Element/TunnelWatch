import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Zahtjevi na /api idu na lokalni backend, pa preglednik vidi jednu adresu i backend ne treba CORS.
    proxy: {
      '/api': {
        target: 'http://localhost:5161',
        changeOrigin: true,
      },
    },
  },
})