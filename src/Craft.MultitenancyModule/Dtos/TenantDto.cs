using Microsoft.VisualBasic;

namespace Craft.MultitenancyModule.Dtos;

public record TenantDto(Guid Id, string Name, string? Host, string? Description, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt);