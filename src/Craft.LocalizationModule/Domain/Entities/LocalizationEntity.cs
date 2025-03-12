namespace Craft.LocalizationModule.Domain.Entities;

public sealed class LocalizationEntity
{
    public Guid Id { get; set; }
    public string CultureCode { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
