# Build the project

1.  change server in `appsettings.json` to your local database.  


2. add a migration with 
```powershell
dotnet ef migrations add AddCartTable
```

3, Update the database
```powershell
dotnet ef database update
```
