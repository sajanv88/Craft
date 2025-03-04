using Craft.Api.Extensions;
using Craft.Api.Infrastructure;
using Craft.Api.Modules;
using Craft.CraftModule;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Craft.KeycloakModule;
using Craft.KeycloakModule.Enums;
using Craft.KeycloakModule.Extensions;
using Craft.KeycloakModule.Options;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var keycloakSettings = builder.Configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>();


builder.Services.AddCraftKeycloakAuthorization();

builder.Services.AddCraftKeycloakAuthentication(options =>
{
    options.Realm = keycloakSettings.Realm;
    options.Audience = keycloakSettings.Audience;
});

builder.Services.AddOpenApiOauth(builder.Configuration);


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
    app.MapScalarApiReference(config =>
    {
        config
            .WithPreferredScheme("OAuth2")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithOAuth2Authentication(o =>
            {
                o.Scopes = ["openid", "profile", "email", "offline_access"];
                o.ClientId = keycloakSettings.ClientId;
            });
    });
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseRouting();

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
        var app = builder.MapGroup("/api");
        app.MapGet("/", () => "Hello from ApiModule!").RequireAuthorization(KeycloakRoles.User.ToString());;
        app.MapGet(
            "/auth",
            (KeycloakModule keycloak, HttpContext ctx) =>
            {
                var u = ctx.User;
                return Results.Ok(new { Data = keycloak.GetModuleName() });
            }).RequireAuthorization();
        return builder;
    }
}