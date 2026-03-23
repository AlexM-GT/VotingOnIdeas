## Plan: Voting App MVP

Build the application as a cleanly separated full-stack MVP: a React 19 SPA in client, a .NET 10 Web API in server, and SQL Server accessed through Entity Framework. Implement the backend first so the authentication model, domain rules, database schema, and HTTP contracts are stable before wiring the frontend. Use JWT access tokens plus refresh tokens, owner-only idea edits/deletes for the MVP, numeric scoring for idea rating, and a paginated ideas list.

**Steps**
1. Phase 1: Solution foundation. Create the backend solution under server with source projects in server/src and test projects in server/tests, and create the frontend app under client with Vite, React 19, TypeScript strict mode, Tailwind CSS, npm, and Playwright. This phase establishes only the project skeletons, shared configuration, environment-variable strategy, and local developer conventions.
2. Phase 1: Backend architecture setup. Create four main backend projects: Domain, Application, Infrastructure, and API. The Domain project owns core entities and business rules. The Application project owns use cases, DTOs, validators, and interfaces. The Infrastructure project owns Entity Framework, SQL Server access, token generation support, and persistence implementations. The API project owns controllers, dependency injection, authentication middleware, exception handling, OpenAPI, and CORS. This blocks all later backend work.
3. Phase 1: Domain model definition. Define the aggregates and relationships for User, Idea, and Vote using the following schema:
   - **Users** — `Id`, `Username`, `Email`, `PasswordHash`, `Salt`, `CreatedAt`; roles: `user` (default) and `admin`.
   - **Ideas** — `Id`, `Title`, `Description`, `UserId` (FK → Users), `CreatedAt`.
   - **Votes** — `Id`, `IdeaId` (FK → Ideas), `UserId` (FK → Users), `Value` (int, 1–5), `CreatedAt`.
   Relationships: User → Ideas (1:many), User → Votes (1:many), Idea → Votes (1:many). Enforce one vote per (UserId, IdeaId) pair so the PUT rating endpoint behaves as an upsert. Capture owner-only authorization (update/delete ideas) as an application rule; admin role bypasses owner check. Keep the role model simple: store role as a string column on Users.
4. Phase 1: Persistence and authentication schema. Add the Entity Framework DbContext, SQL Server provider, entity configurations, and initial migrations. Model user credentials securely with hashed passwords and add refresh-token persistence if refresh tokens are stored server-side. Add indexes and constraints for unique user email and unique user-plus-idea rating pairs. This depends on step 3.
5. Phase 2: Authentication and user flows. Implement registration, login, token refresh, logout, and current-user retrieval in the API and application layers. Use JWT access tokens and refresh tokens as the agreed auth model. Define the request and response contracts early, because the frontend auth context and route protection depend on them. This depends on steps 2 and 4.
6. Phase 2: Idea API surface. Implement the MVP idea endpoints: paginated list, single-item get, create, update, delete, and PUT rating. Keep authorization rules explicit: any authenticated user can create and rate, only the owner can update or delete, and the design leaves space for admin roles later without implementing them now. The rating endpoint should update an existing user rating if present or create one if absent. This depends on steps 3 through 5.
7. Phase 2: Backend quality layer. Add input validation, standardized error responses, structured logging, and OpenAPI documentation for all auth, user, and idea endpoints. Add unit tests for validators, auth flows, ownership checks, CRUD handlers, and rating upsert behavior, following Arrange / Act / Assert. Add targeted integration tests for persistence-heavy flows such as registration, login, idea CRUD, and rating updates. This can run in parallel with late-stage API wiring once contracts are stable.
8. Phase 3: Frontend application shell. Build the React app shell with routing, layout, Tailwind setup, API base URL configuration, and a small service layer over native fetch. Add an auth provider, protected routes, login and registration pages, and session bootstrapping around JWT plus refresh-token behavior. This depends on step 5.
9. Phase 3: Frontend idea workflows. Implement the paginated ideas list, idea detail view, create form, edit form, delete flow, and numeric rating interaction. Shape the UI around the backend contracts rather than duplicating business logic in the client. Show owner-only actions conditionally, and keep the MVP to a straightforward responsive interface rather than broader collaboration or moderation features. This depends on steps 6 and 8.
10. Phase 3: Frontend quality layer. Add component and hook tests where useful, then cover the critical user journey with Playwright: register, log in, list ideas, create an idea, edit the owned idea, rate an idea, delete the owned idea, and log out. Include mobile and desktop checks for the main flows. This depends on steps 8 and 9.
11. Phase 4: Containerized local environment. Add Docker support for frontend, backend, and SQL Server with the backend exposed on port 8080 and the frontend on port 3000. Ensure local environment variables, connection strings, CORS, and startup ordering are documented and work both in local development and in containers. This depends on steps 4, 5, 6, 8, and 9.
12. Phase 4: Delivery documentation. Expand the repository documentation with setup steps, environment variables, migration commands, test commands, container startup, and a short API overview. This should be the final step once the actual commands and file structure are concrete.

**Relevant files**
- d:/WORK/VotingOnIdeas/.github/copilot-instructions.md — repository-wide requirements for stack, testing, and UI expectations
- d:/WORK/VotingOnIdeas/.github/instructions/backend.instructions.md — backend-specific structure, EF, Docker, and test constraints
- d:/WORK/VotingOnIdeas/.github/instructions/frontend.instructions.md — frontend-specific framework, tooling, and Docker constraints
- d:/WORK/VotingOnIdeas/README.md — should be expanded with final setup, environment, migration, and test guidance

**Verification**
1. Confirm the repository structure matches the agreed conventions: backend solution directly under server, backend source under server/src, backend tests under server/tests, and frontend app under client.
2. Verify the backend can start, connect to SQL Server through Entity Framework, apply migrations cleanly, and expose the documented auth and idea endpoints.
3. Verify registration, login, refresh, and logout flows work end to end with the chosen JWT plus refresh-token strategy.
4. Verify the idea list is paginated, idea CRUD respects owner-only edit/delete rules, and the PUT rating endpoint behaves as an upsert for one numeric rating per user per idea.
5. Run backend unit and integration tests covering auth, ownership, CRUD, and rating rules.
6. Run frontend tests and Playwright flows covering the main authenticated user journey on desktop and mobile viewports.
7. Verify Docker-based local startup brings up SQL Server, the .NET API on port 8080, and the React app on port 3000 with working frontend-to-API communication.

**Decisions**
- Included scope: registration, login, authenticated user operations, paginated idea listing, idea CRUD, numeric idea rating via PUT, Entity Framework persistence, testing, and Dockerized local development.
- Excluded scope: admin role implementation, password reset, email confirmation, moderation workflows, real-time updates, search, tagging, categories, attachments, and production deployment infrastructure.
- Auth decision: JWT access tokens plus refresh tokens.
- Authorization decision: owner-only update and delete for ideas, with future extensibility for admins.
- Rating decision: numeric rating model with one rating per user per idea.
- List decision: paginated ideas endpoint and paginated frontend list.
- Delivery order: backend contracts first, frontend integration second, local orchestration and documentation last.

**Further Considerations**
1. Token storage detail should be finalized during implementation kickoff: httpOnly refresh-token cookie with short-lived access token is the safer default for a browser client.
2. Decide early whether to use ASP.NET Core Identity or a lighter custom auth implementation; custom auth is simpler for this MVP, while Identity helps if roles, password recovery, and account management are expected soon.
3. Keep API response contracts stable once the frontend starts, and avoid inventing extra entity fields until the MVP workflow is passing tests end to end.