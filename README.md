# E-Commerce Inventory API

A RESTful API for managing products, categories, and user access in an e-commerce inventory system. Built on .NET 8, it follows a layered Domain-Driven Design and secures business endpoints with JWT authentication.

## Features
- JWT authentication for register and login
- Product CRUD with category filter, price range, pagination, and search
- Category CRUD with uniqueness checks and safe deletion (409 when in use)
- Optional product images via URL or Base64
- Repository + Unit of Work on Entity Framework Core
- Swagger/OpenAPI with Bearer auth
- PostgreSQL or SQL Server via configuration

## Architecture
- `src/ECommerceInventory.API/` — Presentation (controllers, DI, Swagger)
- `src/ECommerceInventory.Application/` — DTOs, services, exceptions
- `src/ECommerceInventory.Domain/` — Entities, repository contracts, base domain types
- `src/ECommerceInventory.Infrastructure/` — EF Core DbContext, repositories, migrations

## Tech Stack
- .NET 8 Web API
- Entity Framework Core
- PostgreSQL 14+ or SQL Server LocalDB
- JWT Bearer auth with BCrypt password hashing
- Swashbuckle for API docs

## Prerequisites
- .NET 8 SDK
- PostgreSQL 14+ or SQL Server LocalDB
- Optional: Visual Studio 2022 or VS Code with C# extension

## Getting Started
1. Clone
   ```bash
   git clone <your-repository-url>
   cd ECommerceInventory
   ```
2. Choose provider
   - Edit `src/ECommerceInventory.API/appsettings.json`
   - Set `DatabaseProvider` to `PostgreSQL` or `SqlServer`
3. Configure connection strings
   - For SQL Server: set `ConnectionStrings:DefaultConnection`
   - For PostgreSQL: set `ConnectionStrings:PostgreSQL`
   ```json
   {
     "DatabaseProvider": "PostgreSQL",
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceInventoryDb;Trusted_Connection=true;",
       "PostgreSQL": "Host=localhost;Port=5432;Database=ECommerceInventoryDb_Dev;Username=postgres;Password=yourpassword"
     }
   }
   ```
4. Set JWT settings in `appsettings.json`
   ```json
   {
     "JWT": {
       "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
       "Issuer": "ECommerceInventoryAPI",
       "Audience": "ECommerceInventoryApp"
     }
   }
   ```
5. Restore and build
   ```bash
   dotnet restore
   dotnet build
   ```
6. Create and apply migrations
   ```bash
   dotnet ef migrations add InitialCreate --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
   dotnet ef database update --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
   ```
7. Run
   ```bash
   dotnet run --project src/ECommerceInventory.API
   ```
   Swagger UI is available in Development at `https://localhost:7164` or `http://localhost:5025`.

## API Surface
### Authentication
- `POST /api/auth/register`
- `POST /api/auth/login`

### Categories
- `GET /api/categories`
- `GET /api/categories/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}` (409 if products depend on it)

### Products
- `GET /api/products` (supports `categoryId`, `minPrice`, `maxPrice`, `page`, `limit`)
- `GET /api/products/search?q=...`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

Authorize in Swagger using `Bearer <your-jwt>` once you obtain a token from the login endpoint.

## Development Commands
```bash
# Build the solution
dotnet build

# Run unit tests (add tests as needed)
dotnet test

# Add a new migration
dotnet ef migrations add <MigrationName> --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API

# Update the database
dotnet ef database update --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API

# Remove the last migration (if not applied)
dotnet ef migrations remove --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
```

## Security Notes
- Passwords are hashed with BCrypt before persistence
- All product and category endpoints require a valid JWT
- CORS is AllowAll for development; restrict for production
- Replace the sample JWT secret with a secure 32+ character value before deployment

## Troubleshooting
1. Connection/provider mismatch: ensure `DatabaseProvider` matches the connection string you configured
2. Migration errors: rename the migration or reset your local DB and retry
3. Swagger auth issues: confirm JWT secret/issuer/audience match between config and code
4. PostgreSQL case-insensitive search: EF `Contains` is case-sensitive with PostgreSQL; consider `citext` or use `ILIKE` via custom queries if needed

