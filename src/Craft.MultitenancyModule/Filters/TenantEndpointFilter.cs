using Craft.MultitenancyModule.Domain.Entities;
using Craft.MultitenancyModule.Extensions;
using Craft.MultitenancyModule.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Craft.MultitenancyModule.Filters;

public class TenantEndpointFilter(
    TenantDbContext dbContext,
    MultitenancyOtions multitenancyOtions,
    ILogger<TenantEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation($"Host setting is set to: {multitenancyOtions.EnableHost}");
        var httpContext = context.HttpContext;
        
        TenantEntity? tenant = null;

        var tenantId = httpContext.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (string.IsNullOrEmpty(tenantId) && !multitenancyOtions.EnableHost)
        {
            logger.LogInformation(
                $"No tenant ID found in the request headers.");


            return Results.BadRequest(new
                { message = "TenantId is missing!, For example: Tenant id set to header X-Tenant-ID" });
        }


        Guid.TryParse(tenantId, out var tenantGuid);

        tenant = await dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == tenantGuid);


        if (tenant is null)
        {
            if (!multitenancyOtions.EnableHost ||
                (!string.IsNullOrEmpty(tenantId) && multitenancyOtions.EnableHost))
                return Results.BadRequest(new { message = "Invalid request." });

            logger.LogInformation(
                $"No tenant found with ID: {tenantId}. Hence, continuing as a HOST.");
            httpContext.Items["connectionString"] = multitenancyOtions.DatabaseConnectionString;
            return await next(context);
        }
        
        logger.LogInformation("Tenant found: {tenantId}", tenant?.Id);

        httpContext.Items["tenantId"] = tenant.Id;
        httpContext.Items["connectionString"] = tenant.ConnectionString;
        httpContext.Items["host"] = tenant.Host;
        return await next(context);
    }
}