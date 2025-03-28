namespace Craft.LocalizationModule.Dtos;

/// <summary>
///   Input request Payload
/// </summary>
/// <param name="CultureCode"></param>
/// <param name="Key"></param>
/// <param name="Value"></param>
public record CreateLocaleDto(string CultureCode, string Key, string Value);
