namespace Craft.LocalizationModule.Dtos;

/// <summary>
///     Contains Locale details
/// </summary>
/// <param name="Id"></param>
/// <param name="CultureCode"></param>
/// <param name="Key"></param>
/// <param name="Value"></param>
public record LocaleDto(Guid Id, string CultureCode, string Key, string Value);
