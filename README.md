# VotingOnIdeas

VotingOnIdeas is a full-stack web application where users can submit ideas, browse ideas from others, and vote on them using a 1-5 rating system. The platform supports user authentication (register, login, refresh, logout), role-based access (user and admin), and idea ownership rules, with admin-level moderation capabilities.

The solution is split into two main parts:

Frontend (client): React 19 + TypeScript + Vite single-page application for authentication, idea browsing, creation, editing, rating, and account actions.
Backend (server): .NET 10 Web API built with clean architecture principles, Entity Framework Core, SQL Server persistence, JWT-based auth, and refresh token flow.
The project is designed for both local development and containerized execution using Docker Compose (frontend, backend, and SQL Server services). It also includes automated quality checks with xUnit for backend tests and Vitest/Playwright for frontend unit and end-to-end testing.

## MCP servers

1. playwright
Purpose: Browser automation and UI testing support through Playwright MCP.
Launch: npx -y @playwright/mcp@latest
Transport: stdio
Practical use here: Helps with interacting with and validating the frontend flows (aligned with your Playwright E2E setup in client).

2. figma
Purpose: Accessing Figma design data/assets through MCP.
Launch: npx -y figma-developer-mcp --stdio --figma-api-key=${input:figmaApiKey}
Auth input: figmaApiKey is requested via promptString with password: true (hidden input).
Practical use here: Used to pull design references from Figma when implementing UI screens/components.

## Tools

Github Copilot in VS Code

## Key prompts

See in [text](docs/prompts_history.md)

## Observations and learnings

1. Using MCP servers makes things easier.
2. Well-prepared guidelines at an early stage simplify the development process. It is better to spend time upfront on guidelines that will be applied from the start than to refactor the code later.
3. It is essential to create a plan. It makes the development process easier. With a well-designed plan, development can essentially become a matter of following a series of ‘execute steps 1–10 from the plan’ commands, with only minor interventions required to adjust the plan itself or the code.
4. UI-related MCP servers may produce unexpectedly different results for the same prompt.
5. ChatGPT creates better plans, whilst Claude Sonet is better at writing code.

## Repository Structure

- `client/` React 19 + TypeScript + Vite frontend
- `server/` .NET 10 solution (`src/` and `tests/`)
- `docs/` planning and supporting documentation

## Prerequisites

- .NET SDK 10
- Node.js 20+
- npm
- SQL Server (local/dev) or Docker
- Docker Desktop (for containerized setup)

## Environment Variables

### Frontend

File: `client/.env.local`

```env
VITE_API_URL=http://localhost:8080/api
```

### Backend (local development)

File: `server/src/VotingOnIdeas.API/appsettings.Development.json`

- `ConnectionStrings:DefaultConnection`
- `Jwt:Secret`

Default dev example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=VotingOnIdeasDb;User Id=sa;Password=sa_P@ssword_1;TrustServerCertificate=True;Encrypt=False;"
  },
  "Jwt": {
    "Secret": "dev-secret-change-this-to-at-least-32-chars!!"
  }
}
```

## Run Locally (Without Docker)

### 1. Start backend

From repository root:

```bash
dotnet run --project server/src/VotingOnIdeas.API
```

Backend URL: `http://localhost:8080`

API docs (Scalar): `http://localhost:8080/scalar/v1`

### 2. Start frontend

From repository root:

```bash
cd client
npm install
npm run dev
```

Frontend URL: `http://localhost:3000`

## Entity Framework Migrations

### Apply migrations to database

```bash
dotnet ef database update --project server/src/VotingOnIdeas.Infrastructure --startup-project server/src/VotingOnIdeas.API
```

### Create a new migration

```bash
dotnet ef migrations add <MigrationName> --project server/src/VotingOnIdeas.Infrastructure --startup-project server/src/VotingOnIdeas.API
```

If `dotnet ef` is missing:

```bash
dotnet tool install --global dotnet-ef
```

## Test Commands

### Backend tests

Run all backend test projects:

```bash
dotnet test server/tests
```

### Frontend unit tests

```bash
cd client
npm run test:unit
```

### Frontend E2E tests (Playwright)

Install browsers once:

```bash
cd client
npx playwright install
```

Run E2E suite:

```bash
cd client
npm run test:e2e
```

## API Overview

### Auth endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout` (authorized)
- `GET /api/auth/me` (authorized)

### Idea endpoints

- `GET /api/ideas?page=1&pageSize=10`
- `GET /api/ideas/{id}`
- `POST /api/ideas` (authorized)
- `PUT /api/ideas/{id}` (authorized)
- `DELETE /api/ideas/{id}` (authorized)
- `PUT /api/ideas/{id}/rating` (authorized)

## Docker Local Environment

Compose file: `docker-compose.yml`

### Services and Ports

1. `frontend`
   Path: `client/Dockerfile`
   Host port: `3000` (override with `FRONTEND_HOST_PORT`)
   Container port: `80`
2. `backend`
   Path: `server/Dockerfile`
   Host port: `8080` (override with `BACKEND_HOST_PORT`)
   Container port: `8080`
3. `sqlserver`
   Image: `mcr.microsoft.com/mssql/server:2022-latest`
   Host port: `1433` (override with `SQL_HOST_PORT`)
   Container port: `1433`

### Key Docker Environment Wiring

- Frontend build arg: `VITE_API_URL=http://localhost:${BACKEND_HOST_PORT:-8080}/api`
- Backend DB connection: `Server=sqlserver,1433;...`
- Backend CORS origin: `http://localhost:${FRONTEND_HOST_PORT:-3000}`
- Backend waits for healthy SQL Server before starting

### Start with defaults

From repository root:

```bash
docker compose up --build
```

### Start with custom host ports

Bash:

```bash
FRONTEND_HOST_PORT=3001 BACKEND_HOST_PORT=8081 SQL_HOST_PORT=14333 docker compose up --build
```

PowerShell:

```powershell
$env:FRONTEND_HOST_PORT='3001'; $env:BACKEND_HOST_PORT='8081'; $env:SQL_HOST_PORT='14333'; docker compose up --build
```

Command Prompt (`cmd`):

```cmd
set "FRONTEND_HOST_PORT=3001" && set "BACKEND_HOST_PORT=8081" && set "SQL_HOST_PORT=14333" && docker compose up --build
```

### Useful compose commands

```bash
docker compose up -d --build
docker compose down
docker compose down -v
docker compose logs -f
docker compose logs -f backend
docker compose logs -f frontend
docker compose logs -f sqlserver
```

## Verification Checklist

### Local

1. Frontend opens: `http://localhost:3000`
2. Backend docs open: `http://localhost:8080/scalar/v1`
3. Register/login/create/rate/delete idea flow works

### Docker (default ports)

1. Frontend opens: `http://localhost:3000`
2. Backend docs open: `http://localhost:8080/scalar/v1`
3. API base responds: `http://localhost:8080/api`

### Docker (overridden ports)

Use your overridden host ports consistently for frontend and backend URLs.

## Troubleshooting

1. Port already allocated
   Use `FRONTEND_HOST_PORT`, `BACKEND_HOST_PORT`, `SQL_HOST_PORT` overrides.
2. CORS errors in Docker
   Ensure backend CORS origin matches frontend host port.
3. SQL startup timing
   Backend already waits on SQL healthcheck, but first start can still take longer.
4. Stale volume/schema issues
   Run `docker compose down -v` and start again.
5. Build cache issues
   Run `docker compose build --no-cache`.