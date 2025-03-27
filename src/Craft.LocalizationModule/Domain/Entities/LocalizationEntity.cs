namespace Craft.LocalizationModule.Domain.Entities;

/// <summary>
///     Entity for localization
/// </summary>
public sealed class LocalizationEntity
{
    /// <summary>
    ///   Id of the entity
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///  Culture code of the entity
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;

    /// <summary>
    ///    Key of the entity
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     Value of the entity
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
