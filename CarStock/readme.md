# CarStock API

A robust C# Web API designed to help car dealers manage their local vehicle inventories securely. Built with .NET 9, Fast-Endpoints, SQLite, and Dapper.

## Requirements

- [.NET 9 SDK]
- Docker (Optional, for containerized running)

## How to Run the Project

### Option 1: Running Natively
1. Open your terminal and navigate to the `CarStock` directory:
   ```bash
   cd CarStock
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to the Swagger UI:
   ```
   http://localhost:5237/swagger
   ```

### Option 2: Running with Docker
You can easily spin up the application using Docker Compose, which maps port `5237` to your host machine.

1. Navigate to the `CarStock` directory.
2. Build and start the container in the background:
   ```bash
   docker-compose up --build -d
   ```
3. Access the API via Swagger at:
   ```
   http://localhost:5237/swagger
   ```
*(Note: The `database.db` SQLite file is mounted as a volume so your data persists even if the container restarts).*

## API Endpoints

Once running, you can interact with the API via the Swagger UI or via HTTP requests. **All endpoints (except login) require a Bearer token in the `Authorization` header.**

### Authentication
* **`POST /auth/login`**: Acts as both a Login and Registration endpoint. Send a JSON body with `Username` and `Password`. If the user doesn't exist, it creates the account automatically and returns a valid JWT token.

### Inventory Management
* **`GET /cars`**: Retrieves a paginated list of all cars currently in the logged-in dealer's garage. 
  * Query Params: `Page` (default 1), `Size` (default 20)
* **`POST /cars`**: Adds a new car to the dealer's garage.
  * Body fields: `Make`, `Model`, `Year`, `StockLevel`
* **`GET /cars/search`**: Searches for cars in the dealer's garage by Make and/or Model.
  * Query Params: `Make`, `Model`, `Page`, `Size`
* **`PATCH /cars/{CarId}/stock`**: Directly updates the stock level of an existing car.
  * URL Param: `CarId`
  * Body fields: `CarId`, `StockLevel`
* **`DELETE /car/{CarId}`**: Removes a car from the dealer's garage.
  * URL Param: `CarId`

## Design Details & Implementation Notes

### Database Configuration (SQLite)
This project uses a local SQLite database (`database.db`) to fulfill the database requirement efficiently without needing heavy external SQL server setups.
- **Auto-Initialization**: You do not need to run any manual migrations or SQL scripts. On application startup, the application automatically ensures the `Dealers`, `Cars`, and `Garages` tables are created, along with optimal indexes.
- **ORM-Free Data Access**: Querying is performed using raw SQL strings mapped safely via **Dapper** in the Repository layer (`CarRepository`, `DealerRepository`), which protects against SQL injection while demonstrating raw SQL proficiency.

### Authentication & Tenant Isolation
- **Strict Data Isolation**: The API is strictly multi-tenant. The logged-in `DealerId` is extracted securely from the JWT claims on every request. Dealers can only View, Add, Update (Stock Level), or Delete cars that belong to their specific `DealerId` in the `Garages` relationship table. No dealer can access another dealer's inventory.

### Input Validation & Stability
Input validation is handled smoothly using `FluentValidation` seamlessly integrated with Fast-Endpoints. Invalid requests (such as negative stock levels or skipping the car Make) are intercepted automatically by the framework, returning a clean `400 Bad Request` with exact failure reasons.
