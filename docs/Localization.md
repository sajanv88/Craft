# Craft.LocalizationModule

The localizationModule in Craft is a utility designed to handle text localization and internationalization (i18n) within your project. It provides methods for loading, managing, 
and retrieving translated strings based on the user's locale or language preferences.

## Getting Started
1. Install the `dotnet add package Craft.LocalizationModule` NuGet package in your existing Craft API project.

Configure the localization extensions in your `Program.cs` file for example:
```csharp

builder.Services.AddCraftLocalization(options =>
{
    options.SupportedCultureCodes = [];
});

builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);

```
Optionally, you can configure the localization options for example:
```csharp
builder.Services.AddCraftLocalization(options =>
{
    options.SupportedCultureCodes = ["en-US", "nl-NL", "ta-IN"]; // provide supported culture codes in your app. You can get this information by calling /all-cultures endpoint. See below.
    options.PolicyName = "api"; // Define your own policy name and configure it in your app. So, Localization Module can be validated by this policy.
    options.EndpointContextPath = "/api/locales" // Define your own endpoint context path. By default, it is /api/locales.
});
```

2. Mark localization module as a dependency in your main module. For example:
```csharp

[DependsOn(typeof(LocalizationModule))]
public sealed class ApiModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGroup("/api");
        app.MapGet("/", () => "Hello from ApiModule!");
        return builder;
    }
}

```

3. Configure database.
   Localization module uses `LocalizationDbContext` to store localization data and uses PostgreSQL as a default database provider so, in api project
   You have `AppDbContext.cs` file. You need to invoke `builder.ConfigureLocation` method. For example: 
```csharp

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : CraftDbContext<AppDbContext>(options)
{
     // You can add your own dbsets here.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureLocalization(); // Add this line
    }
}


```

4. Run migrations and update the database. For example
```bash

dotnet ef migrations add AddLocalization --project Craft.Api

dotnet ef database update --project Craft.Api
```

Run your application and navigate to the `/api/locales/*` endpoint. Localization module exposes default endpoints as follows.
```text
    HTTP: GET /api/locales
          GET /api/locales/{id}
          PUT /api/locales
          PATCH /api/locales
          DELETE /api/locales/{id}
          GET /api/locales/all-cultures
          GET /api/locales/culture{code}
```

For more details run the demo project inside this repository. It has a demonstration of Craft extensions around a common ASP.NET Core Web API project. Along with Scalar playground. 
It has a localization module example. Also, payload and responses are documented.


