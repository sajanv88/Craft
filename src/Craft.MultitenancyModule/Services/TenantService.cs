using System.Text.RegularExpressions;
using Craft.CraftModule.Constants;
using Craft.CraftModule.Dtos;
using Craft.MultitenancyModule.Domain.Entities;
using Craft.MultitenancyModule.Dtos;
using Craft.MultitenancyModule.Extensions;
using Craft.MultitenancyModule.Infrastructure;
using Craft.MultitenancyModule.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Craft.MultitenancyModule.Services;

public sealed class TenantService(
    MultitenancyOtions multitenancyOtions,
    TenantDbContext tenantDbContext,
    ILogger<TenantService> logger) : ITenantService
{
    private string GetConnectionString(string newDbName)
    {
        var defaultConnection = multitenancyOtions.DatabaseConnectionString
                                ?? throw new NullReferenceException(
                                    "DatabaseConnectionString for database is missing!");

        return Regex.Replace(defaultConnection, @"(?<=Database=)[^;]+", newDbName);
    }

    public async Task<PaginatedResponse<TenantDto>> GetTenantsAsync(int? page, int? pageSize, string? search = null,
        CancellationToken cancellationToken = default)
    {
        var cp = page ?? 0;
        var limit = pageSize ?? 10;
        var tenants = await tenantDbContext.Tenants.AsNoTracking() .Skip(cp * limit)
            .Take(limit).ToListAsync(cancellationToken);
        var totalCount = await tenantDbContext.Tenants.AsNoTracking().CountAsync(cancellationToken);
        var tenantDtos = tenants.Select(t => new TenantDto(t.Id, t.Name, t.Host, t.Description, t.CreatedAt,
            t.UpdatedAt)).ToList().AsReadOnly();
        return new PaginatedResponse<TenantDto>
        {
            Items = tenantDtos,
            TotalCount = totalCount,
            CurrentPage = cp,
            PageSize = limit
        };
    }

    public Task<TenantDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTenantAsync(Guid id, UpdateTenantDto updateTenant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TenantExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TenantDto?> GetTenantByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> CreateTenantDatabaseAsync(CreateTenantDto newTenant,
        CancellationToken cancellationToken = default)
    {
        var tenantName = newTenant.Name;
        var newDbName = $"Tenant_{tenantName.Replace(" ", "_")}";

        var defaultConnection = multitenancyOtions.DatabaseConnectionString
                                ?? throw new NullReferenceException(
                                    "DatabaseConnectionString for database is missing!");

        var newDbConnectionString = newTenant.ConnectionString ?? GetConnectionString(newDbName);

        await using (var connection = new NpgsqlConnection(defaultConnection))
        {
            await connection.OpenAsync(cancellationToken);

            // Create database command
            var createDbCommand = new NpgsqlCommand($"CREATE DATABASE \"{newDbName}\"", connection);
            await createDbCommand.ExecuteNonQueryAsync(cancellationToken);
            logger.LogInformation($"Created a new database: {newDbName}");
        }


        var qualifiedName = multitenancyOtions.DbContextType.AssemblyQualifiedName;
        await new DbContextTypeResolver().MigrateAsync(qualifiedName, newDbConnectionString, cancellationToken);
        

        var tenant = new TenantEntity()
        {
            Name = tenantName,
            ConnectionString = newDbConnectionString,
            Host = newTenant.Host,
            Description = newTenant.Description,
        };

        await tenantDbContext.Tenants.AddAsync(tenant, cancellationToken);
        await tenantDbContext.SaveChangesAsync(cancellationToken);

        await using (var connection = new NpgsqlConnection(newDbConnectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var tableName = CraftModuleConstants.PrefixAsCraft + "_tenants";
            var sql = $"DROP TABLE IF EXISTS \"{tableName}\" CASCADE;";
            var dropTableCommand = new NpgsqlCommand(sql, connection);
            await dropTableCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        logger.LogInformation($"Tenant {tenantName} created with ID: {tenant.Id}");
        return tenant.Id;
    }
}