namespace Craft.LocalizationModule.Dtos;

public record LocaleWithCultureDetailDto(CultureCodeAndDetailDto CultureDetails, Dictionary<string, string> L);