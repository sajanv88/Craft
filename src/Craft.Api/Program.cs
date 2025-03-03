using Craft.Api.Infrastructure;
using Craft.Api.Modules;
using Craft.CraftModule;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Craft.KeycloakModule;
using Craft.KeycloakModule.Extensions;
using Craft.KeycloakModule.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCraftModulesFromAssembly(typeof(Program).Assembly);



builder.Services.AddCraftKeycloakAuthentication(options =>
{ 
    
    var keycloakSettings = builder.Configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>();
    
    options.Realm = keycloakSettings.Realm;
    options.Audience = keycloakSettings.Audience;
    options.Authority = keycloakSettings.Authority;
    options.VerifyTokenAudience = keycloakSettings.VerifyTokenAudience;
    
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapCraftModulesEndpoint();

app.Run();

[DependsOn(typeof(KeycloakModule))]
public sealed class ApiModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGroup("/api").RequireAuthorization();
        app.MapGet("/", () => "Hello from ApiModule!");
        app.MapGet(
            "/auth",
            (KeycloakModule keycloak) => keycloak.GetModuleName()
        );
        return builder;
    }
}
