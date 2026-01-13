# IMS.Data — Persistence helpers

This project contains a small EF Core-backed generic repository and `ApplicationDbContext`.

Usage (example from `IMS.WebAPI` Program.cs):

```csharp
using IMS.Data.DependencyInjection;

builder.Services.AddPersistence(options =>
{
    // For local development with SQL Server:
    // options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));

    // For quick testing, use in-memory provider:
    options.UseInMemoryDatabase("IMS-Dev");
});
```

Registering will add `ApplicationDbContext` and open generic `IGenericRepository<T>` -> `EfRepository<T>` to DI.
