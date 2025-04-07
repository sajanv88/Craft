namespace Craft.MultitenancyModule.Dtos;

public record CreateTenantDto(string Name, string? Description, string? Host, string? ConnectionString);
