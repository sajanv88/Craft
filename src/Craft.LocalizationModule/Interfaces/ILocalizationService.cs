using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Dtos;

namespace Craft.LocalizationModule.Interfaces;

/// <summary>
///     Interface for the Localization Service, which contains the methods for the Localization Module
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    ///  Returns a paginated list of locales
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="cultureCode"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PaginatedResponse<LocaleDto>> GetLocalizationsAsync(
        int? page,
        int? pageSize,
        string? cultureCode = null,
        string? key = null,
        string? value = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Returns a locale by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LocaleDto?> GetLocalizationAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Creates a new locale
    /// </summary>
    /// <param name="createLocaleDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Guid> CreateLocalesAsync(
        CreateLocaleDto createLocaleDto,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Updates an existing locale
    /// </summary>
    /// <param name="updateLocaleDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<LocaleDto> UpdateLocalesAsync(
        UpdateLocaleDto updateLocaleDto,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Deletes a locale by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task DeleteLocalesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Returns a list of all cultures
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<CultureCodeAndDetailDto> ListAllCultures();

    /// <summary>
    ///     Returns a list of all cultures with their details
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<LocaleWithCultureDetailDto>? GetCultureDetailAsync(string code);
}
