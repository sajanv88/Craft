

## Craft
Craft is the ultimate framework that supercharges ASP.NET Core! With its sleek layer of extension methods and powerful features, it transforms your code into a modular, clean, and effortlessly simple masterpiece. Build smarter, not harder!

For a better understanding, take a look at the `Craft.Api` project inside this repo. I have a demonstration usages of Craft extensions around common ASP.NET Core Web Api project.

#### Routing
Craft leverages the power of `IEndpointRouteBuilder` routing and harnesses all the extensions from `IEndpointConventionBuilder` aka Minimal APIs to keep your code sleek and efficient. Need a secure route? No problem! Define a route with built-in authorization in just a few lines. Clean, modern, and developer-friendly!


### Get Started with Craft in Minutes! �

```bash
dotnet new webapi --output web-api
cd web-api && dotnet add package Craft.CraftModule
```

Open the `web-api` project in your favorite IDE and create a new module, e.g., `TodoModule.cs`.

Open `Program.cs` and call the Craft extension method to register your module(s):

```csharp
// Option 1: Automatically discover modules from the assembly
builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);

// Option 2: Manually specify modules
builder.Services.AddCraftModules([typeof(TodoModule)]);
```
Choose whichever suits your style!


Finally, add this line to automatically map all your module endpoints:
```csharp
app.UseCraftGeneralException(); //  general exception handler (optional)

app.MapCraftModulesEndpoint();
```
And that’s it! Your endpoints are now ready to roll. 🚀



### Simple todo api example

```csharp

public sealed class Todo
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public sealed class TodoModule : CraftModule
{
    private List<Todo> _todos = new List<Todo>
    {
        new Todo { Id = 1, Title = "Buy milk" },
        new Todo { Id = 2, Title = "Walk the dog" },
    };
    
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    {
        var endpoints = builder.MapGroup("/api/todos");
        endpoints.MapGet("/", () => _todos);
        endpoints.MapGet("/{id}", (int id) => _todos.FirstOrDefault(x => x.Id == id));
        endpoints.MapPut("/", (string title) => 
        {
            var todo = new Todo { Id = _todos.Count + 1, Title = title };
            _todos.Add(todo);
            return Results.Created($"/api/todos/{todo.Id}", todo);
        });
        endpoints.MapDelete("/{id}", (int id) => 
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                return Results.NotFound();
            }
            _todos.Remove(todo);
            return Results.NoContent();
        });
        endpoints.MapPatch("/{id}", (int id, string title) => 
        {
            var todo = _todos.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                return Results.NotFound();
            }
            todo.Title = title;
            return Results.Ok(todo);
        });
        return builder;
    }
}
```
### Want to Use a Database in Your Module?
Craft has you covered! You can seamlessly integrate Entity Framework Core into your module.

#### Install the PostgreSQL Provider
To get started, install the Npgsql.EntityFrameworkCore.PostgreSQL package:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```
#### Create a Database Context
Next, create an ApiDbContext class that inherits from CraftDbContext and define a DbSet for your entity.

`ApiDbContext.cs`

```csharp

using Craft.Api.Domain;
using Craft.CraftModule.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Craft.Api.Infrastructure;

public class ApiDbContext(DbContextOptions<ApiDbContext> options)
    : CraftDbContext<ApiDbContext>(options)
{
    public DbSet<TodoEntity> Todos { get; set; }
}

```
#### Configure Your Entity in the Module
In your `TodoModule.cs` file, override the ConfigureModelBuilder method to define your entity schema.

```csharp
public sealed class TodoModule : CraftModule
{
    public override void ConfigureModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });
    }
}
```
#### Define Your Connection String
Add your database connection string to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=craft;Username=postgres;Password=postgres"
  }
}
```
#### Configure the Database Context in `Program.cs`
Modify your `Program.cs` file to register the database context:

```csharp
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});


```
#### Run Migrations and Update the Database
```bash

dotnet ef migrations add InitialCreate
dotnet ef database update

```

🎉 That's It!
Your module is now fully integrated with Entity Framework Core.

For a complete working example, check out the example `Craft.Api` project in this repository under the Modules folder. 🚀

Additionally, you can use a separate **DbContext** for each module.

For example, simply create a `TodoDbContext` and register it in the `PreInitialization` method:

```csharp
services.AddDbContext<TodoDbContext>();
```  

This allows each module to have its own isolated database context, improving modularity and maintainability. 🚀


### Craft Available Modules

- Craft.CraftModule
- Craft.KeycloakModule
  - [How to use Craft.KeycloakModule](docs/Keycloak.md)
- Craft.LocalizationModule
  - [How to use Craft.LocalizationModule](docs/Localization.md)

### Future Plans
- Craft.LocalizationModule
- Craft.AIAgentModule
- Craft.BackgroundTaskModule

### Contributing

Craft is an open-source project and welcomes contributions from the community.
If you have any ideas, suggestions, or improvements, feel free to open an issue or submit a pull request. 
Let’s make Craft the best framework for ASP.NET Core together!

### Author

- [Sajan](https://github.com/sajanv88)


### License
Craft is licensed under the MIT license. See the [LICENSE](LICENSE) file for more information.

