using Microsoft.VisualBasic;

namespace Craft.MultitenancyModule.Domain.Entities;

public sealed class TenantEntity
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; } 
    
    public string? Host { get; set; }
    
    public string? ConnectionString { get; set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}
