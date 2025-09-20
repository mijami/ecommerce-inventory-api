# E-Commerce Inventory API

A RESTful API for managing products, categories, and user access in an e-commerce inventory system. The solution is built on .NET 8, follows a layered Domain-Driven Design structure, and secures every business endpoint with JWT authentication.

## Features
- JWT authentication for user registration and login
- Product CRUD with category filtering, price-range filtering, pagination, and search
- Category CRUD with uniqueness checks and safe deletion rules
- Optional product image storage via URL or Base64 metadata
- Swagger/OpenAPI documentation with security definitions
- Repository + Unit of Work patterns on top of Entity Framework Core
- Works with SQL Server or PostgreSQL via configuration

## Architecture
`
src/
+-- ECommerceInventory.API/           # Presentation layer (controllers, IoC, swagger)
+-- ECommerceInventory.Application/   # DTOs, services, cross-layer exceptions
+-- ECommerceInventory.Domain/        # Entities and repository contracts
+-- ECommerceInventory.Infrastructure/# EF Core context and repository implementations
`

## Tech Stack
- .NET 8 Web API
- Entity Framework Core
- SQL Server LocalDB or PostgreSQL
- JWT Bearer authentication with BCrypt password hashing
- Swashbuckle.AspNetCore for API docs

## Prerequisites
- .NET 8 SDK
- SQL Server LocalDB **or** PostgreSQL 14+
- Optional: Visual Studio 2022 / VS Code with C# extension

## Getting Started
1. **Clone the repository**
   `ash
   git clone <your-repository-url>
   cd ECommerceInventory
   `
2. **Select the database provider**
   - Open src/ECommerceInventory.API/appsettings.json (and the Development variant if needed).
   - Set "DatabaseProvider" to "SqlServer" or "PostgreSQL".
3. **Configure connection strings**
   - DefaultConnection is used for SQL Server.
   - PostgreSQL is used when the provider is set to PostgreSQL.
   `json
   "DatabaseProvider": "PostgreSQL",
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceInventoryDb;Trusted_Connection=true;",
     "PostgreSQL": "Host=localhost;Database=ECommerceInventoryDb;Username=postgres;Password=yourpassword"
   }
   `
4. **Update JWT settings** (replace the secret, issuer, and audience as needed):
   `json
   "JWT": {
     "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
     "Issuer": "ECommerceInventoryAPI",
     "Audience": "ECommerceInventoryApp"
   }
   `
5. **Restore dependencies**
   `ash
   dotnet restore
   `
6. **Create and apply migrations**
   `ash
   dotnet ef migrations add InitialCreate --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
   dotnet ef database update --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
   `
7. **Run the API**
   `ash
   dotnet run --project src/ECommerceInventory.API
   `
   Swagger UI is available at https://localhost:7071 by default.

## API Surface
### Authentication
- POST /api/auth/register
- POST /api/auth/login

### Categories
- GET /api/categories
- GET /api/categories/{id}
- POST /api/categories
- PUT /api/categories/{id}
- DELETE /api/categories/{id} *(returns 409 if products depend on the category)*

### Products
- GET /api/products with optional categoryId, minPrice, maxPrice, page, limit
- GET /api/products/search?q=
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}

Authorize in Swagger using the Bearer scheme once you obtain a JWT from the login endpoint.

## Development Commands
`ash
# Build the solution
dotnet build

# Run unit tests (add tests under the solution as needed)
dotnet test

# Add a new migration
dotnet ef migrations add <MigrationName> --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API

# Update the database
dotnet ef database update --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API

# Remove the last migration (if not applied)
dotnet ef migrations remove --project src/ECommerceInventory.Infrastructure --startup-project src/ECommerceInventory.API
`

## Security Notes
- Passwords are hashed with BCrypt before persistence.
- All product and category endpoints require a valid JWT.
- CORS is currently open (AllowAll) for convenience; tighten it for production.
- Always replace the sample JWT secret with a secure 32+ character value in production.

## Troubleshooting
1. **Connection issues**: Ensure the provider value matches the connection string you have configured.
2. **Migration errors**: Delete the local database or change the migration name and retry.
3. **JWT errors in Swagger**: Verify the secret, issuer, and audience match between configuration and code.
4. **PostgreSQL casing in search**: For case-insensitive search, enable citext or apply ILIKE via custom queries.