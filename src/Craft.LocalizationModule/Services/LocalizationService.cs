using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Interfaces;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Craft.LocalizationModule.Services;

public sealed class LocalizationService(
    LocalizationDbContext dbContext,
    LocalizationConfiguration localizationConfiguration,
    ILogger<LocalizationService> logger,
    IValidator<CreateLocaleDto> validator)
    : ILocalizationService
{
    public async Task<PaginatedResponse<LocaleDto>> GetLocalizationsAsync(int? page, int? pageSize,
        string? cultureCode = null, string? key = null, string? value = null,
        CancellationToken cancellationToken = default)
    {
        var cp = page ?? 0;
        var limit = pageSize ?? 10;
        logger.LogInformation(
            $"Getting localizations... Page: {cp}, PageSize: {limit}, CultureCode: {cultureCode ?? null}, Key: {key ?? null}, Value: {value ?? null}");


        var queryable = dbContext.Localizations
            .AsNoTracking();


        if (localizationConfiguration.SupportedCultureCodes.Count > 0)
        {
            logger.LogInformation(
                $"Filtering by supported culture codes {string.Join(",", localizationConfiguration.SupportedCultureCodes)}");
            queryable = queryable.Where(l
                => localizationConfiguration.SupportedCultureCodes.Contains(l.CultureCode));
        }


        if (!string.IsNullOrWhiteSpace(cultureCode))
        {
            queryable = queryable.Where(x => x.CultureCode == cultureCode);
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            queryable = queryable.Where(x => x.Key == key);
        }

        if (!string.IsNullOrWhiteSpace(value))
        {
            queryable = queryable.Where(x => x.Value == value);
        }

        var total = await queryable.CountAsync(cancellationToken);
        var data = await queryable.Skip(cp * limit).Take(limit).ToListAsync(cancellationToken);
        var results = data.ToList().AsReadOnly();
        logger.LogInformation($"Found {results.Count} localizations");

        return new PaginatedResponse<LocaleDto>
        {
            Items = results.Select(x => new LocaleDto(x.Id, x.CultureCode, x.Key, x.Value)).ToList().AsReadOnly(),
            TotalCount = total,
            CurrentPage = cp,
            PageSize = limit
        };
    }

    public async Task<LocaleDto?> GetLocalizationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Getting localization.. Id: {id}");
        var locale = await dbContext.Localizations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return locale is null ? null : new LocaleDto(locale.Id, locale.CultureCode, locale.Key, locale.Value);
    }

    public async Task<Guid> CreateLocalesAsync(CreateLocaleDto createLocaleDto,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(createLocaleDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for CreateLocaleDto");
            throw new ValidationException(validationResult.Errors);
        }


        // Check if the key already exists
        var existing =
            dbContext.Localizations
                .FirstOrDefault(x => x.Key == createLocaleDto.Key && x.CultureCode == createLocaleDto.CultureCode);
        if (existing != null)
        {
            logger.LogWarning(
                $"Duplicate localization key: {createLocaleDto.Key} for culture code: {createLocaleDto.CultureCode}");
            throw new InvalidOperationException(
                $"The key '{createLocaleDto.Key}' already exists for culture code '{createLocaleDto.CultureCode}'");
        }

        var entity = new LocalizationEntity
        {
            CultureCode = createLocaleDto.CultureCode,
            Key = createLocaleDto.Key,
            Value = createLocaleDto.Value
        };
        dbContext.Localizations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<LocaleDto> UpdateLocalesAsync(UpdateLocaleDto updateLocaleDto,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            new CreateLocaleDto(updateLocaleDto.CultureCode, updateLocaleDto.Key, updateLocaleDto.Value),
            cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for UpdateLocaleDto");
            throw new ValidationException(validationResult.Errors);
        }


        var locale = await dbContext.Localizations
            .FirstOrDefaultAsync(x => x.Id == updateLocaleDto.Id, cancellationToken);

        if (locale is null)
        {
            logger.LogWarning($"No localization found for id: {updateLocaleDto.Id}");
            throw new InvalidOperationException($"The locale with id '{updateLocaleDto.Id}' was not found");
        }

        locale.CultureCode = updateLocaleDto.CultureCode;
        locale.Key = updateLocaleDto.Key;
        locale.Value = updateLocaleDto.Value;
        dbContext.Localizations.Update(locale);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new LocaleDto(locale.Id, locale.CultureCode, locale.Key, locale.Value);
    }

    public async Task DeleteLocalesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var locale = await dbContext.Localizations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (locale is null)
        {
            logger.LogWarning($"No localization found for id: {id}");
            throw new InvalidOperationException($"The locale with id '{id}' was not found");
        }

        dbContext.Localizations.Remove(locale);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IReadOnlyList<CultureCodeAndDetailDto> ListAllCultures()
    {
        var dic = CultureInfo.AllCultures;
        return dic.Select(x => new CultureCodeAndDetailDto(x.Key, x.Value)).ToList().AsReadOnly();
    }

    public async Task<LocaleWithCultureDetailDto>? GetCultureDetailAsync(string code)
    {
        var culture = CultureInfo.AllCultures.FirstOrDefault(x => x.Key == code);
        if (culture.Key is null) return null;

        var locales = await dbContext.Localizations.AsNoTracking().Where(l => l.CultureCode == code).ToListAsync();
        var dic = locales.ToDictionary(x => x.Key, x => x.Value);
        return new LocaleWithCultureDetailDto(new CultureCodeAndDetailDto(culture.Key, culture.Value),
            dic);
    }
}