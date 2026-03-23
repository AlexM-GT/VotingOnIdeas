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
