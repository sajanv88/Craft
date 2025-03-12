namespace Craft.LocalizationModule.Dtos;

public record UpdateLocaleDto(Guid Id, string CultureCode, string Key, string Value);