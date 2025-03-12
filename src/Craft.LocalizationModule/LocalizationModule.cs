using System.Reflection;
using Craft.CraftModule.Dtos;
using Craft.CraftModule.Infrastructure;
using Craft.LocalizationModule.Domain.Entities;
using Craft.LocalizationModule.Domain.Interfaces;
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
    public LocalizationModule(IConfiguration configuration, ILogger<LocalizationModule> logger)
    {
        _configuration = configuration;
        _logger = logger;
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
        var endpoint = builder.MapGroup("/api/locales").WithTags("Locales");
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
        return builder;
    }
}

