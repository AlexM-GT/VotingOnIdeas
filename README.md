# VotingOnIdeas

VotingOnIdeas is a full-stack MVP with:
- React SPA frontend (`client`)
- .NET 10 Web API backend (`server`)
- SQL Server database

## Docker Local Environment (Step 11)

This repository includes a complete Docker Compose setup for frontend, backend, and SQL Server.

### Services and Ports

1. `frontend`
	Path: `client/Dockerfile`
	Exposed host port: `3000` (override with `FRONTEND_HOST_PORT`)
	Container port: `80` (Nginx)
2. `backend`
	Path: `server/Dockerfile`
	Exposed host port: `8080` (override with `BACKEND_HOST_PORT`)
	Container port: `8080`
3. `sqlserver`
	Image: `mcr.microsoft.com/mssql/server:2022-latest`
	Exposed host port: `1433` (override with `SQL_HOST_PORT`, for example `14333`)

Compose file: `docker-compose.yml`

### Key Environment Wiring

1. Frontend API base URL in containers:
	`VITE_API_URL=http://localhost:${BACKEND_HOST_PORT:-8080}/api`
	Note: this value is correct for browser calls from your host machine.
2. Backend SQL connection string in containers:
	`ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=VotingOnIdeasDb;User Id=sa;Password=sa_P@ssword_1;TrustServerCertificate=True;Encrypt=False;`
3. Backend JWT secret in containers:
	`Jwt__Secret=dev-secret-change-this-to-at-least-32-chars!!`
4. Backend CORS origin in containers:
	`Cors__AllowedOrigins__0=http://localhost:${FRONTEND_HOST_PORT:-3000}`

### Startup Ordering

1. `sqlserver` starts first and publishes a healthcheck.
2. `backend` waits for healthy SQL Server (`depends_on` with `service_healthy`).
3. `frontend` starts after backend container start.

### Run with Docker

From repo root (`d:\WORK\VotingOnIdeas`):

```bash
docker compose up --build
```

If ports are already used on your machine:

```bash
FRONTEND_HOST_PORT=3001 BACKEND_HOST_PORT=8081 SQL_HOST_PORT=14333 docker compose up --build
```

Run detached:

```bash
docker compose up -d --build
```

Stop and remove containers:

```bash
docker compose down
```

Stop and remove containers and DB volume:

```bash
docker compose down -v
```

### Verify the Stack

1. Frontend: `http://localhost:3000`
2. API docs: `http://localhost:8080/scalar/v1`
3. API base: `http://localhost:8080/api`

### Logs and Troubleshooting

1. Follow all logs:
	`docker compose logs -f`
2. Backend logs:
	`docker compose logs -f backend`
3. SQL logs:
	`docker compose logs -f sqlserver`
4. Frontend logs:
	`docker compose logs -f frontend`

If startup fails:
1. Ensure host ports are free for frontend, backend, and SQL Server.
	If not, run with `FRONTEND_HOST_PORT`, `BACKEND_HOST_PORT`, and `SQL_HOST_PORT` overrides.
2. Rebuild images:
	`docker compose build --no-cache`
3. Reset data and retry:
	`docker compose down -v` then `docker compose up --build`