

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
app.MapCraftModulesEndpoint();
```
And that’s it! Your endpoints are now ready to roll. 🚀

