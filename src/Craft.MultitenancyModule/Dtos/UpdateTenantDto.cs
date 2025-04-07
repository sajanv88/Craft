namespace Craft.MultitenancyModule.Dtos;

public record UpdateTenantDto(string Name, string? Description, string? Host, string? ConnectionString);
