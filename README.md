# Expense Tracker API

REST API for tracking income and expenses, built with ASP.NET Core 9, Entity Framework Core and MySQL.

## Features
- JWT authentication (register, login) with BCrypt password hashing
- Categories and transactions CRUD, scoped to the logged-in user
- Server-side filtering, sorting and paging
- Monthly summary with spending by category
- Global error handling, CORS, health check, OpenAPI docs (Scalar)

## Tech stack
ASP.NET Core 9, EF Core 9 (Pomelo MySQL), MySQL 8, JWT Bearer

## Getting started

1. Create the database:
   ```sql
   CREATE DATABASE expense_tracker CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
2. Set secrets:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Port=3306;Database=expense_tracker;User=root;Password=YOUR_PASSWORD;"
   dotnet user-secrets set "Jwt:Key" "A_RANDOM_KEY_AT_LEAST_32_CHARACTERS"
   ```
3. Apply migrations and run:
   ```bash
   dotnet ef database update
   dotnet run
   ```
4. Open the docs at `http://localhost:5074/scalar/v1`

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | /api/auth/register | Create an account |
| POST | /api/auth/login | Get a JWT |
| GET/POST | /api/categories | List / create categories |
| PUT/DELETE | /api/categories/{id} | Update / delete a category |
| GET/POST | /api/transactions | List (filter, page) / create |
| PUT/DELETE | /api/transactions/{id} | Update / delete a transaction |
| GET | /api/summary?month=&year= | Monthly totals and category breakdown |
| GET | /health | Health check |

## Security notes
- User ID always comes from the JWT, never from the request body
- Every query is filtered by the logged-in user
- Passwords are hashed with BCrypt
