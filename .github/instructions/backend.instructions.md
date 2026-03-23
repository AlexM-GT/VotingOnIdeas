---
applyTo: 'server/**'
description: 'Backend-specific instructions for the server folder: structure, .NET stack, testing, Docker, and code style.'
---

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
- Expose port 8080 by default.