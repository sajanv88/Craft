namespace Craft.LocalizationModule.Dtos;

/// <summary>
///     Contains current locale details with culture details
/// </summary>
/// <param name="CultureDetails"></param>
/// <param name="L"></param>
public record LocaleWithCultureDetailDto(
    CultureCodeAndDetailDto CultureDetails,
    Dictionary<string, string> L
);
