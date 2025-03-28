namespace Craft.LocalizationModule.Dtos;

/// <summary>
///     Input request Payload
/// </summary>
/// <param name="Id"></param>
/// <param name="CultureCode"></param>
/// <param name="Key"></param>
/// <param name="Value"></param>
public record UpdateLocaleDto(
    Guid Id,
    string CultureCode,
    string Key,
    string Value
);
