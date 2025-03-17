using Craft.CraftModule.Dtos;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Interfaces;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Infrastructure;
using Craft.LocalizationModule.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Craft.LocalizationModule;

public  class LocalizationModule : CraftModule.CraftModule
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalizationModule> _logger;
    private readonly LocalizationConfiguration _localesConfig;
    public LocalizationModule(IConfiguration configuration, ILogger<LocalizationModule> logger, LocalizationConfiguration localesConfig)
    {
        _configuration = configuration;
        _logger = logger;
        _localesConfig = localesConfig;
    }

    public LocalizationModule()
    {
        
    }
    public override void PreInitialization(IServiceCollection services)
    {
        
        services.AddDbContext<LocalizationDbContext>(o =>
        {
            o.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<ILocalizationService, LocalizationService>();
        
    }

    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        var endpoint = builder.MapGroup(_localesConfig.EndpointContextPath);
        if (!string.IsNullOrWhiteSpace(_localesConfig.PolicyName))
        {
            _logger.LogInformation("Policy {PolicyName} is configured", _localesConfig.PolicyName);
            endpoint = builder.MapGroup(_localesConfig.EndpointContextPath)
                .RequireAuthorization(_localesConfig.PolicyName);
        }
        
        var scope = builder.ServiceProvider.CreateScope();
        var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        endpoint.MapGet("/", localizationService.GetLocalizationsAsync)
            .Produces<PaginatedResponse<LocalizationEntity>>();
        endpoint.MapPut("/", localizationService.CreateLocalesAsync).Produces<Guid>();
        endpoint.MapGet("/{id}", localizationService.GetLocalizationAsync)
            .Produces<LocalizationEntity>();
        endpoint.MapPatch("/", localizationService.UpdateLocalesAsync)
            .Produces<LocalizationEntity>();
        endpoint.MapDelete("/{id}", localizationService.DeleteLocalesAsync);
        endpoint.MapGet("/all-cultures", localizationService.ListAllCultures)
            .Produces<IReadOnlyList<CultureCodeAndDetailDto>>();
        endpoint.MapGet("/culture/{code}", localizationService.GetCultureDetail)
            .Produces<CultureCodeAndDetailDto>();
        
        
        return builder;
    }
}

