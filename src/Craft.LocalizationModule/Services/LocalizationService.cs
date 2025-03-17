using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Interfaces;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Craft.LocalizationModule.Services;


public sealed class LocalizationService(LocalizationDbContext dbContext,
    LocalizationConfiguration localizationConfiguration,
    ILogger<LocalizationService> logger)
    : ILocalizationService
{
    
    public async Task<PaginatedResponse<LocalizationEntity>> GetLocalizationsAsync(int? page, int? pageSize,
        string? cultureCode = null, string? key = null, string? value = null,
        CancellationToken cancellationToken = default)
    {

        var cp = page ?? 0;
        var limit = pageSize ?? 10;
        logger.LogInformation($"Getting localizations... Page: {cp}, PageSize: {limit}, CultureCode: {cultureCode ?? null}, Key: {key ?? null}, Value: {value ?? null}");
        

        var queryable = dbContext.Localizations
            .AsNoTracking();
        
        
        if (localizationConfiguration.SupportedCultureCodes.Count > 0)
        {
            logger.LogInformation($"Filtering by supported culture codes {string.Join(",", localizationConfiguration.SupportedCultureCodes)}");
            queryable = queryable.
                Where(l 
                    => localizationConfiguration.SupportedCultureCodes.Contains(l.CultureCode))
                .Skip(cp * limit)
                .Take(limit);
        }
        else
        {
            queryable = queryable
                .Skip(cp * limit)
                .Take(limit);
        }
        
        if (!string.IsNullOrWhiteSpace(cultureCode))
        {
            queryable = queryable.Where(x => x.CultureCode == cultureCode)
                .Skip(cp * limit)
                .Take(limit);
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            queryable = queryable.Where(x => x.Key == key)
                .Skip(cp * limit)
                .Take(limit);
        }

        if (!string.IsNullOrWhiteSpace(value))
        {
            queryable = queryable.Where(x => x.Value == value)
                .Skip(cp * limit)
                .Take(limit);
        }

        var total = await queryable.CountAsync(cancellationToken);
        var data = await queryable.ToListAsync(cancellationToken);
        var results = data.ToList().AsReadOnly();
        logger.LogInformation($"Found {results.Count} localizations");

        return new PaginatedResponse<LocalizationEntity>
        {
            Items = results,
            TotalCount = total,
            CurrentPage = cp,
            PageSize = limit
        };
    }

    public Task<LocalizationEntity?> GetLocalizationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Getting localization.. Id: {id}");
        return dbContext.Localizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Guid> CreateLocalesAsync(CreateLocaleDto createLocaleDto,
        CancellationToken cancellationToken = default)
    {
        var entity = new LocalizationEntity
        {
            CultureCode = createLocaleDto.CultureCode,
            Key = createLocaleDto.Key,
            Value = createLocaleDto.Value
        };
        // Check if the key already exists
        var existing = dbContext.Localizations.FirstOrDefault(x => x.Key == entity.Key && x.CultureCode == entity.CultureCode);
        if (existing != null)
        {
            logger.LogWarning($"Duplicate localization key: {entity.Key} for culture code: {entity.CultureCode}");
            throw new InvalidOperationException($"The key '{entity.Key}' already exists for culture code '{entity.CultureCode}'");
        }

        await dbContext.Localizations.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<LocalizationEntity> UpdateLocalesAsync(UpdateLocaleDto updateLocaleDto,
        CancellationToken cancellationToken = default)
    {
        var locale = await GetLocalizationAsync(updateLocaleDto.Id, cancellationToken);

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
        return locale;
    }

    public async Task DeleteLocalesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var locale = await GetLocalizationAsync(id, cancellationToken);

        if (locale is null)
        {
            logger.LogWarning($"No localization found for id: {id}");
            throw new InvalidOperationException($"The locale with id '{id}' was not found");
        }
        
        dbContext.Localizations.RemoveRange(locale);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public IReadOnlyList<CultureCodeAndDetailDto> ListAllCultures()
    {
        var dic = CultureInfo.AllCultures;
        return dic.Select(x => new CultureCodeAndDetailDto(x.Key, x.Value)).ToList().AsReadOnly();
    }

    public CultureCodeAndDetailDto? GetCultureDetail(string code)
    {
        var culture = CultureInfo.AllCultures.FirstOrDefault(x => x.Key == code);
        return culture.Key is null ? null : new CultureCodeAndDetailDto(culture.Key, culture.Value);
    }
}
