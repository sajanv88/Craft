

## Craft
Craft is the ultimate framework that supercharges ASP.NET Core! With its sleek layer of extension methods and powerful features, it transforms your code into a modular, clean, and effortlessly simple masterpiece. Build smarter, not harder!

For a better understanding, take a look at the `Craft.Api` project inside this repo. I have a demonstration usages of Craft extensions around common ASP.NET Core Web Api project.

### Routing
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

You want to use database EntityFramework core? No problem! Craft has you covered. 
Just inherit from `CraftDbContext` and add your entities. Here’s an example:

```csharp
// AppDbContext.cs

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : CraftDbContext<AppDbContext>(options)
{
    public DbSet<TodoEntity> Todos { get; set; }
}


builder.Services.AddDbContext<AppDbContext>(o =>
{
    var connectionString = builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );
    o.UseNpgsql(connectionString);
});

// Configure ModelBuilder for your entities in your Module. For example:

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

Finally, add this line to automatically map all your module endpoints:
```csharp
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

### Contributing

Craft is an open-source project and welcomes contributions from the community.
If you have any ideas, suggestions, or improvements, feel free to open an issue or submit a pull request. 
Let’s make Craft the best framework for ASP.NET Core together!

### Author

- [Sajan](https://github.com/sajanv88)

### License
Craft is licensed under the MIT license. See the [LICENSE](LICENSE) file for more information.
