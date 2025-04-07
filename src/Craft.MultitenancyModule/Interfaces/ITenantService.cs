using Craft.CraftModule.Dtos;
using Craft.MultitenancyModule.Dtos;

namespace Craft.MultitenancyModule.Interfaces;

public interface ITenantService
{
    public Task<PaginatedResponse<TenantDto>> GetTenantsAsync(
        int? page,
        int? pageSize,
        string? search = null,
        CancellationToken cancellationToken = default);
    
    public Task<TenantDto?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task UpdateTenantAsync(Guid id, UpdateTenantDto updateTenant, CancellationToken cancellationToken = default);
    public Task DeleteTenantAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<bool> TenantExistsAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<TenantDto?> GetTenantByNameAsync(string name, CancellationToken cancellationToken = default);
    
    public Task<Guid> CreateTenantDatabaseAsync(CreateTenantDto newTenant, CancellationToken cancellationToken = default);
}