# TunnelWatch Backend

ASP.NET Core Web API (.NET 10) that subscribes to the CAN App MQTT messages from the SICK TDC-E gateway, stores the decoded signals in PostgreSQL and exposes them over a REST API.

## Requirements

- .NET 10 SDK
- Docker Desktop
- Visual Studio 2026 (18.x)

## Run

In `Software/Backend`:

```bash
docker compose up -d
dotnet run --project src/TunnelWatch.Api
```

- `docker compose` starts Mosquitto (port 1883) and PostgreSQL 18 (port 5433, user, password and database `tunnelwatch`).
- In Development, database migrations are applied automatically at startup.
- Swagger UI: http://localhost:5161/swagger
- Request examples: `src/TunnelWatch.Api/TunnelWatch.Api.http`

## REST API

| Endpoint | Response |
|---|---|
| `GET /api/gateways` | 200, gateways and the time of their last message, sorted by id |
| `GET /api/gateways/{id}/latest` | 200, latest value, unit and timestamp of every signal, sorted by name. 404 "Nema podataka za uređaj" if there is no data |

## Configuration

`src/TunnelWatch.Api/appsettings.Development.json`:

| Key | Default | Meaning |
|---|---|---|
| `ConnectionStrings:Default` | `Host=localhost;Port=5433;...` | PostgreSQL connection |
| `Mqtt:Host`, `Mqtt:Port` | `localhost`, `1883` | MQTT broker |
| `Mqtt:TopicFilter` | `+/+/+` | Subscription filter, matches `{gateway}/{port}/{profile}` for all gateways |

The backend reconnects to the broker automatically every 5 s if it is unavailable or the connection drops.

## Storage

| Table | Key | Content |
|---|---|---|
| `gateways` | `id` | time of the last received message |
| `latest_signal_values` | `gateway_id`, `signal_name` | value, unit, timestamp |
| `signal_history` | `gateway_id`, `signal_name`, `minute` | average, minimum, maximum, sample count, unit (one row per signal per minute, UTC) |

## Tests

```bash
dotnet test
```

Parser tests run against the recorded device messages in `samples/`, aggregation tests run without a database.

## Replay recorded messages

See `samples/README.md`. In PowerShell the inner quotes must be escaped:

```powershell
docker cp samples\device-truck.txt backend-mosquitto-1:/tmp/s.txt
docker exec backend-mosquitto-1 sh -c 'while true; do while read -r t p; do mosquitto_pub -t \"$t\" -m \"$p\"; sleep 1; done < /tmp/s.txt; done'
```

Check the stored data:

```powershell
docker exec backend-postgres-1 psql -U tunnelwatch -d tunnelwatch -c "select * from gateways"
```

## Migrations

Requires the `dotnet-ef` tool (`dotnet tool install -g dotnet-ef`). To add a migration after changing the model:

```bash
dotnet ef migrations add <Name> --project src/TunnelWatch.Infrastructure --startup-project src/TunnelWatch.Api --output-dir Persistence/Migrations
```

## Project structure

| Project | Content |
|---|---|
| `TunnelWatch.Domain` | domain models: signal, gateway, latest value, minute history |
| `TunnelWatch.Application` | message parser, storage and query interfaces |
| `TunnelWatch.Infrastructure` | MQTT subscription, EF Core DbContext, storage, migrations |
| `TunnelWatch.Api` | controllers, DTOs, service registration |
| `tests/TunnelWatch.Application.Tests` | xUnit tests |