# Taskify – .NET Consolidated Server (Taskify.NetService)

This folder contains a consolidated .NET 8 API server mirroring the Taskify API, packaged under `Taskify.ApiServer/` with projects for API, Model, and Service. It is functionally equivalent to the API in `Taskify.Api/` with the same ports and auth setup.

## Quick Start
```bash
# from Taskify.NetService/Taskify.ApiServer/
dotnet restore
 dotnet build
 dotnet run --project Taskify.Api/Taskify.Api.csproj
```

- Swagger UI: https://localhost:7292/swagger (or http://localhost:5014/swagger)
- Default ports: HTTPS 7292, HTTP 5014 (see Taskify.Api/Properties/launchSettings.json)

## Design Overview
- Controllers: Users and Tasks endpoints (attribute routing, Swagger enabled).
- Services: `ITaskService`/`TaskService`, `IUserService`/`UserService` for business logic.
- Model: Shared entities and DTOs in `Taskify.Model/`.
- Data: EF Core SQL Server (DbContext) configured via connection string.
- Validation: FluentValidation wired into MVC pipeline.
- Auth: JWT bearer (issuer/audience/key from configuration).
- CORS: local dev origins for Angular (4200) and React (5173).

## Configuration
- Connection string: `ConnectionStrings:DefaultConnection` (
  Azure SQL example with AAD: `Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;Encrypt=True;Authentication=Active Directory Default;`)
- JWT: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` in `appsettings*.json`.
- Ports: managed by launch profiles in `Taskify.Api/Properties/launchSettings.json`.

## Tips
- If running behind Angular dev proxy, trust the ASP.NET HTTPS dev certificate when prompted.
- Update CORS origins in `Program.cs` if you change client ports.
- You can open the solution with Visual Studio using `Taskify.ApiServer/Taskify.Api.slnx`.
