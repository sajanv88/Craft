namespace Craft.LocalizationModule.Dtos;

public record LocaleDto(Guid Id, string CultureCode, string Key, string Value);