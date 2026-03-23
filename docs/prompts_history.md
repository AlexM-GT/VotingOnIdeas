# Prompts History

## 2026-03-23

### Entry 1
- User Prompt: "All prompts must be saved into docs/prompts_history.md file. Information about agent and model must be saved as well as agent response."
- Agent: GitHub Copilot
- Model: GPT-5.3-Codex
- Agent Response: "Implemented. I created `docs/prompts_history.md` and recorded this prompt with agent name, model, and response. I will continue appending future prompts and responses to this file in the same format."

### Entry 2
- User Prompt: "Add Playwright and Figma MCP server configurations. Figma api key must be requested as an input. Input must be hidden like a password."
- Agent: GitHub Copilot
- Model: GPT-5.3-Codex
- Agent Response: "Implemented. Added `.vscode/mcp.json` with Playwright and Figma MCP server configurations. The Figma API key is requested via an input (`promptString`) with `password: true`, so it is hidden when entered."

### Entry 3
- User Prompt: "Create copilot repository wide instructions file. Add next content there. ---
description: \"Repository-wide instructions for the VotingOnIdeas project: coding standards, project guidelines, and AI assistant behaviors.\"
---

# Project Overview
This project is a web application built using React, .NET, and Microsoft SQL Server for data storage. It provides users with a platform to generate ideas and vote on them. The application is designed to be user-friendly and responsive, ensuring a seamless experience across different types of devices.

## Folder Structure
- **/client**: Contains the React frontend code.
- **/server**: Contains the .NET backend code.
- **/docs**: Contains documentation related to the project.

## Libraries and Frameworks
- React: Used for building the user interface of the application.
- .NET: Used for building the backend API that handles business logic and data processing.
- Microsoft SQL Server: Used for data storage and management.
- Entity Framework: Used for object-relational mapping (ORM) in the .NET backend.

## Coding Standards
- Use single quotes for strings in JavaScript.
- Use function based components in React.
- Follow the SOLID principles and best practises in the .NET backend.
- Write unit tests for both frontend and backend code.
- Use meaningful variable and function names to enhance code readability.
- Ensure proper error handling and logging in the backend.

## UI guidelines
- Application should be responsive and work well on both desktop and mobile devices.
- Application should have a modern and clean design, with a focus on usability.
- Use consistent styling and theming across the application."
- Agent: GitHub Copilot
- Model: GPT-5.3-Codex
- Agent Response: "Implemented. Created repository-wide instructions in `.github/copilot-instructions.md` with the exact content provided, including frontmatter and all project/coding/UI guidelines."

### Entry 4
- User Prompt: "Create backend instructions file with the next content: applyTo 'server/**'

# Backend Instructions

## Structure
- The solution file lives directly inside server/ .
- The source code is placed in server/src/ folder .
- The tests are placedin server/tests/ folder .

## Tech stack
- Runtime: .Net 10
- Database: MS SQL Server
- Testing: xUnit + FluentAssertion
- Containerisation: Docker
- Architecture: Clean architecture

Unit tests should be generated automatically for new code, or existing tests should be updated to reflect changes in the code. Arrange / Act / Assert pattern must be followed in the unit tests.
EntityFramework is used to access data in the database.

## Code style
- Use file-scoped namespaces.
- Do not use #region blocks.
- Use sealed on classes that are not designed for inheritance.
- Prefer records for DTOs and result types.
-Async methods must return Task or Task<T> and have the Async suffix.

## Docker
- The Dockerfile lives at 'server/Dockerfile'. 
- Expose port 8080 by default."
- Agent: GitHub Copilot
- Model: GPT-5.4
- Agent Response: "Implemented. Created backend-scoped instructions in `.github/instructions/backend.instructions.md` with `applyTo: 'server/**'` and the provided backend guidance content."

### Entry 5
- User Prompt: "Create frontend instructions file with the next content: applyTo 'client/**'

# Frontend Instructions

## Tech stack
- Framework: React 19
- Language: TypeScript (strict mode)
- Buid tool: Vite
- Styling: Tailwind CSS
- HTTP client: Native fetch API
- E2E  testing: Playwright
- Package manager: npm

## Docker
- The Dockerfile lives at 'client/Dockerfile'.
- Expose port 3000 by default."
- Agent: GitHub Copilot
- Model: GPT-5.4
- Agent Response: "Implemented. Created frontend-scoped instructions in `.github/instructions/frontend.instructions.md` with `applyTo: 'client/**'` and the provided frontend guidance content."

### Entry 6
- User Prompt: "Put the Plan into docs/plan.md file"
- Agent: GitHub Copilot
- Model: GPT-5.4
- Agent Response: "Implemented. Wrote the saved application development plan into `docs/plan.md` and kept the prompt history updated."

### Entry 7
- User Prompt: "Execute Step 1 of the plan"
- Agent: GitHub Copilot
- Model: Claude Sonnet 4.6
- Agent Response: "Implemented. Scaffolded the full Step 1 foundation: (1) Backend — created `server/VotingOnIdeas.sln`, four source projects (Domain, Application, Infrastructure, API) in `server/src/`, three xUnit test projects in `server/tests/`, added all to the solution, wired clean-architecture project references (Application→Domain, Infrastructure→Domain+Application, API→Application+Infrastructure, plus test-to-source references). All 7 projects build cleanly. (2) Frontend — created `package.json` (React 19, TypeScript, Vite, Tailwind CSS, React Router, Playwright), `tsconfig.json`/`tsconfig.app.json`/`tsconfig.node.json` in strict mode, `tailwind.config.js`, `postcss.config.js`, `playwright.config.ts` targeting port 3000 with desktop and mobile Chromium projects, `.env.local` with `VITE_API_BASE_URL`, `index.html`, `src/main.tsx`, `src/App.tsx`, and `src/index.css` with Tailwind directives. Ran `npm install` — 0 vulnerabilities."

### Entry 8
- User Prompt: "Create .gitignore file based on the app React+API structure."
- Agent: GitHub Copilot
- Model: Claude Sonnet 4.6
- Agent Response: "Implemented. Created `.gitignore` at the repo root covering: .NET/ASP.NET Core (bin, obj, .vs, NuGet, build artifacts, test results, secrets/appsettings.Local.json), React/Vite/Node (node_modules, dist, .env*.local, Playwright report folders, tsbuildinfo), and general OS/editor files (macOS, Windows, JetBrains, VS Code — preserving mcp.json and other useful .vscode files)."

### Entry 9
- User Prompt: "Execute Step 2 of the plan"
- Agent: GitHub Copilot
- Model: Claude Sonnet 4.6
- Agent Response: "Implemented. Step 2 backend architecture setup is complete — all 7 projects build cleanly. (1) NuGet packages: Application ← FluentValidation + DI extensions; Infrastructure ← EF Core SQL Server + Tools + Design + JWT Bearer + BCrypt.Net-Next; API ← Serilog.AspNetCore + Serilog.Sinks.Console; all test projects ← FluentAssertions; API.Tests ← Microsoft.AspNetCore.Mvc.Testing. (2) Boilerplate removed: WeatherForecast.cs, WeatherForecastController.cs, all Class1.cs files, .http file. (3) Domain layer: Entity<TId> base class, IRepository<TEntity,TId> interface, IUnitOfWork interface, DomainException. (4) Application layer: Result<T>/Result result pattern, PagedResult<T> record, application exceptions (NotFoundException, UnauthorizedException, ConflictException, ValidationException), DependencyInjection.cs registering FluentValidation validators. (5) Infrastructure layer: AppDbContext with ApplyConfigurationsFromAssembly, UnitOfWork, DependencyInjection.cs wiring EF Core SQL Server + JWT Bearer + IUnitOfWork. (6) API layer: ExceptionHandlingMiddleware mapping all exception types to RFC 9110 ProblemDetails; Program.cs with Serilog, AddApplication(), AddInfrastructure(), CORS, authentication, authorization, OpenAPI; appsettings.json with Serilog/JWT/CORS/ConnectionStrings structure; appsettings.Development.json with local dev defaults."

### Entry 10
- User Prompt: "Update plan step 3 with the next info about db schemas - Users (Id, Username, Email, PasswordHash, Salt, CreatedAt), Ideas (Id, Title, Description, UserId, CreatedAt), Votes (Id, IdeaId, UserId, Value, CreatedAt). Vote value can contain numbers from 1 to 5. User can have 'admin' and 'user' roles."
- Agent: GitHub Copilot
- Model: Claude Sonnet 4.6
- Agent Response: "Implemented. Updated plan step 3 with the concrete DB schema: Users (Id, Username, Email, PasswordHash, Salt, CreatedAt; roles: user/admin), Ideas (Id, Title, Description, UserId, CreatedAt), Votes (Id, IdeaId, UserId, Value 1–5, CreatedAt). Added relationship details, unique (UserId, IdeaId) vote constraint, and admin role bypass rule."

### Entry 11
- User Prompt: "Execute Step 3 of the plan"
- Agent: GitHub Copilot
- Model: Claude Sonnet 4.6
- Agent Response: "Implemented. Step 3 domain model definition is complete — all 7 projects build cleanly in 4.4s. (1) Updated Entity<TId> base to match schema (removed UpdatedAt). (2) Domain/Constants/UserRole.cs — admin/user string constants. (3) Domain/Entities/User.cs — factory method Create(), AssignRole(), navigation to Ideas and Votes collections. (4) Domain/Entities/Idea.cs — factory Create(), Update() method, Vote navigation. (5) Domain/Entities/Vote.cs — factory Create(), UpdateValue(), MinValue/MaxValue constants (1–5), domain validation. (6) Domain/Interfaces: IUserRepository (GetByEmail, GetByUsername, ExistsBy*), IIdeaRepository (GetPagedAsync), IVoteRepository (GetByIdeaAndUserAsync). (7) Infrastructure EF configurations: UserConfiguration (unique email+username indexes, cascade delete to Ideas, no-action for Votes), IdeaConfiguration (length constraints, cascade delete to Votes), VoteConfiguration (unique (UserId,IdeaId) index, CK_Votes_Value check constraint 1–5). (8) AppDbContext updated with DbSet<User>, DbSet<Idea>, DbSet<Vote>. (9) Infrastructure repositories: generic Repository<TEntity,TId> base (virtual GetByIdAsync), UserRepository, IdeaRepository (overrides GetByIdAsync with Include, GetPagedAsync), VoteRepository. (10) DependencyInjection.cs updated to register IUserRepository, IIdeaRepository, IVoteRepository."
