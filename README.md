# README 

## DataStorageTestTask

## Overview
DataStorageTestTask is a C# project using **ASP.NET Core**, **Entity Framework Core**, and **Repository Pattern** to manage customer, project, role, service, and staff data. This project consists of a **Business Layer**, **Data Layer**, and **Web API Layer**.

## Project Structure
```plaintext
DataStorageTestTask/
├── Business/
│   ├── Dtos/
│   ├── Factories/
│   ├── Interfaces/
│   ├── Services/
│   ├── Business.csproj
├── Data/
│   ├── Contexts/
│   ├── Entities/
│   ├── Interfaces/
│   ├── Repositories/
│   ├── Migrations/
│   ├── Data.csproj
├── WebApi/
│   ├── Controllers/
│   ├── Properties/
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Program.cs
│   ├── WebApi.csproj
├── .gitattributes
├── .gitignore
├── DataStorageTestTask.sln
```

## Prerequisites
Ensure you have the following installed:
- [.NET SDK (latest version)](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio](https://visualstudio.microsoft.com/)
- Entity Framework Core CLI:

  ```sh
  dotnet tool install --global dotnet-ef
  ```

## Setting Up Local Database

### 1. Update Connection String
Modify `appsettings.Development.json` to set up your local SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DataStorageDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

> Replace `localhost` with your SQL Server instance if needed.

### 2. Apply Migrations & Update Database
Run the following commands to set up the database:

```sh
# Navigate to the Data project
cd Data

# Add migration (if modifying database schema)
dotnet ef migrations add InitialCreate

# Apply migrations to database
dotnet ef database update
```

## Running the Project

### 1. Start the Web API
Run the API using:

```sh
cd WebApi

# Run the API
 dotnet run
```

### 2. Test the API
Use **Postman** or **Swagger** to test the API at:

```
https://localhost:<port>/swagger/index.html
```

## Adding a New Migration
If you make changes to the entity models, update the database schema:

```sh
cd Data

dotnet ef migrations add <MigrationName>

dotnet ef database update
```

## API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| GET | `/api/projects` | Get all projects |
| POST | `/api/projects` | Create a new project |
| PUT | `/api/projects/{id}` | Update a project |
| DELETE | `/api/projects/{id}` | Delete a project |

## Contributing
1. Fork the repository.
2. Create a feature branch: `git checkout -b feature-name`
3. Commit your changes: `git commit -m "Added feature"`
4. Push to the branch: `git push origin feature-name`
5. Open a pull request.

## License
This project is licensed under the MIT License.