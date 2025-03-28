using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Infrastructure;
using Craft.LocalizationModule.Interfaces;
using Craft.LocalizationModule.Services;
using Craft.LocalizationModule.validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Craft.LocalizationModule;

/// <summary>
///  Craft LocalizationModule class. This class is used to configure the localization module
/// </summary>
public class LocalizationModule : CraftModule.CraftModule
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalizationModule> _logger;
    private readonly LocalizationConfiguration _localesConfig;

    /// <summary>
    ///     Constructor for the LocalizationModule
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="localesConfig"></param>
    public LocalizationModule(
        IConfiguration configuration,
        ILogger<LocalizationModule> logger,
        LocalizationConfiguration localesConfig
    )
    {
        _configuration = configuration;
        _logger = logger;
        _localesConfig = localesConfig;
    }

    /// <summary>
    /// Default constructor for the LocalizationModule
    /// </summary>
    public LocalizationModule() { }

    /// <summary>
    ///    PreInitialization method for the LocalizationModule. This method is used to configure the services for the module
    /// </summary>
    /// <param name="services"></param>
    public override void PreInitialization(IServiceCollection services)
    {
        services.AddDbContext<LocalizationDbContext>(o =>
        {
            o.UseNpgsql(
                _configuration.GetConnectionString("DefaultConnection")
            );
        });

        services.AddScoped<ILocalizationService, LocalizationService>();
        services.AddScoped<
            IValidator<CreateLocaleDto>,
            CreateLocaleDtoValidator
        >();
    }

    /// <summary>
    ///     Add routes for the LocalizationModule
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var endpoint = builder.MapGroup(_localesConfig.EndpointContextPath);
        if (!string.IsNullOrWhiteSpace(_localesConfig.PolicyName))
        {
            _logger.LogInformation(
                "Policy {PolicyName} is configured",
                _localesConfig.PolicyName
            );
            endpoint = builder
                .MapGroup(_localesConfig.EndpointContextPath)
                .RequireAuthorization(_localesConfig.PolicyName);
        }

        var scope = builder.ServiceProvider.CreateScope();
        var localizationService =
            scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        endpoint
            .MapGet("/", localizationService.GetLocalizationsAsync)
            .Produces<PaginatedResponse<LocaleDto>>();
        endpoint
            .MapPut("/", localizationService.CreateLocalesAsync)
            .Produces<Guid>(StatusCodes.Status201Created);
        endpoint
            .MapGet("/{id}", localizationService.GetLocalizationAsync)
            .Produces<LocalizationEntity>();
        endpoint
            .MapPatch("/", localizationService.UpdateLocalesAsync)
            .Produces<LocalizationEntity>();
        endpoint.MapDelete("/{id}", localizationService.DeleteLocalesAsync);
        endpoint
            .MapGet("/all-cultures", localizationService.ListAllCultures)
            .Produces<IReadOnlyList<CultureCodeAndDetailDto>>();
        endpoint
            .MapGet(
                "/culture/{code}",
                localizationService.GetCultureDetailAsync
            )
            .Produces<LocaleWithCultureDetailDto>();

        return builder;
    }
}
