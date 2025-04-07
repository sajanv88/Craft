using Craft.MultitenancyModule.Dtos;
using Craft.MultitenancyModule.Extensions;
using Craft.MultitenancyModule.Infrastructure;
using Craft.MultitenancyModule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.MultitenancyModule;

public class MultitenancyModule : CraftModule.CraftModule
{
    private readonly IConfiguration _configuration;

    public MultitenancyModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MultitenancyModule()
    {
    }

    public override void PreInitialization(IServiceCollection services)
    {
        services.AddDbContext<TenantDbContext>(o =>
        {
            o.UseNpgsql(
                _configuration.GetConnectionString("DefaultConnection")
            );
        });
    }

    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        var multitenancyConfiguration = builder.ServiceProvider.GetRequiredService<MultitenancyOtions>();
        var policyName = multitenancyConfiguration.PolicyName;
        var endpoint = builder.MapGroup("/api/tenants").WithTags("Tenants");
        
        if(!string.IsNullOrEmpty(policyName))
        {
            endpoint.RequireAuthorization(policyName);
        }
        
        endpoint.MapGet("/", async (TenantDbContext dbContext) =>
        {
            var tenants = await dbContext.Tenants
                .Select(t => new TenantDto(t.Id, t.Name, t.Host, t.Description, t.CreatedAt, t.UpdatedAt))
                .ToListAsync();
            return Results.Ok(tenants);
        });

        endpoint.MapPut("/", async ([FromBody] CreateTenantDto tenantDto, ITenantService tenantService) =>
        {
            var id = await tenantService.CreateTenantDatabaseAsync(tenantDto);
            return Results.Ok(new { TenantId = id });
        });
        return endpoint;
    }
}