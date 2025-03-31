set ASPNETCORE_ENVIRONMENT=Development
dotnet ef database drop --force
rmdir /s /q Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update