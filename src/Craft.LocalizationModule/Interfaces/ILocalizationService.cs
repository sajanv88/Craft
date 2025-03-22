using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Dtos;

namespace Craft.LocalizationModule.Interfaces;

public interface ILocalizationService
{
    public Task<PaginatedResponse<LocaleDto>> GetLocalizationsAsync(int? page, int? pageSize, string? cultureCode = null, string? key = null, string? value = null, CancellationToken cancellationToken = default);
    public Task<LocaleDto?> GetLocalizationAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Guid> CreateLocalesAsync(CreateLocaleDto createLocaleDto, CancellationToken cancellationToken = default);
    public Task<LocaleDto> UpdateLocalesAsync(UpdateLocaleDto updateLocaleDto, CancellationToken cancellationToken = default);
    public Task DeleteLocalesAsync(Guid id, CancellationToken cancellationToken = default);

    public IReadOnlyList<CultureCodeAndDetailDto> ListAllCultures();
    
    public Task<LocaleWithCultureDetailDto>? GetCultureDetailAsync(string code);

}
