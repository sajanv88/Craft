using Craft.CraftModule.Extensions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);

// Minimal setup for testing
app.MapCraftModulesEndpoint(); // Map the endpoints
app.Run();

public partial class Program { }
