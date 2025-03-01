using Craft.Api.Infrastructure;
using Craft.CraftModule;
using Craft.CraftModule.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);


builder.Services.AddDbContext<ApiDbContext>(o =>
{
    var connectionString = builder.Configuration.GetConnectionString(
        "DefaultConnection"
    );
    o.UseNpgsql(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCraftModulesEndpoint();

app.Run();

public sealed class ApiModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGet("/", () => "Hello World!");
        return builder;
    }
}
