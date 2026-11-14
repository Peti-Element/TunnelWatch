# TunnelWatch Web

Web application for the site manager: a list of machines and the latest values of their signals. Built with React, TypeScript and Vite; data comes from the TunnelWatch backend (`GET /api/gateways`, `GET /api/gateways/{id}/latest`).

## Requirements

- Node.js (LTS) with npm
- Running backend for live data (see `Software/Backend/README.md`)

### Windows PowerShell

By default PowerShell blocks scripts, so `npm` fails with "running scripts is disabled on this system". Either:

- allow local scripts once for your user (recommended):

  ```powershell
  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
  ```

- or use `npm.cmd` (and `npx.cmd`) instead of `npm` in every command:

  ```powershell
  npm.cmd install
  npm.cmd run dev
  ```

In Command Prompt (`cmd`) `npm` works without any change.

## Run

In `Software/Web`:

```bash
npm install
npm run dev
```

Open http://localhost:5173.

Requests to `/api` are forwarded by the Vite dev server to the local backend at http://localhost:5161 (see `vite.config.ts`), so the backend needs no CORS settings. Start the backend first:

```bash
cd ../Backend
docker compose up -d
dotnet run --project src/TunnelWatch.Api
```

## Routes

| Route | Page |
|---|---|
| `/` | redirects to `/machines` |
| `/machines` | list of machines |
| `/machines/:id` | latest signal values of one machine |

## Scripts

| Command | What it does |
|---|---|
| `npm run dev` | development server with hot reload |
| `npm run build` | type check and production build into `dist/` |
| `npm run lint` | lint with Oxlint |
| `npm run preview` | serve the production build locally |

## Structure

| Path | Content |
|---|---|
| `src/main.tsx` | entry point, router |
| `src/App.tsx` | routes |
| `src/components/` | shared components (layout) |
| `src/pages/` | one component per route |
| `vite.config.ts` | Vite config, `/api` proxy |