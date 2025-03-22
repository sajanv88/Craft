using System.Text;
using System.Text.Json;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Craft.LocalizationModule.Infrastructure;
using Craft.LocalizationModule.Interfaces;
using Craft.LocalizationModule.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.LocalizationModuleTests;

[DependsOn(typeof(LocalizationModule.LocalizationModule))]
public sealed class LocalizationModuleTest : CraftModule.CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        return builder;
    }
}

public class CraftLocalizationModuleTest : IClassFixture<TestDatabaseFixture>, IAsyncDisposable
{
    private readonly TestDatabaseFixture _fixture;
    private readonly LocalizationDbContext _dbContext;
    private readonly WebApplicationBuilder _webApplicationBuilder;
    private readonly WebApplication _webApplication;
    public CraftLocalizationModuleTest(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", _fixture.ConnectionString }
        };
        Console.WriteLine($"Using database connection string: {_fixture.ConnectionString}");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _dbContext = fixture.DbContext;
        _webApplicationBuilder = WebApplication.CreateBuilder();
        
  
        var services = _webApplicationBuilder.Services;
        services.AddHttpClient("client", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000");
        });
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddCraftLocalization(option =>
        {
            option.SupportedCultureCodes = [];
        });
      
        CraftModuleExtensions.InitializedModules.Clear();
        services.AddCraftModules([typeof(LocalizationModuleTest)]);

        _webApplication = _webApplicationBuilder.Build();
        _webApplication.MapCraftModulesEndpoint();
        _webApplication.RunAsync();
    }

    [Fact(DisplayName = "Craft Localization Module default endpoints are registered.")]
    public async Task Test1()
    {
        var endpointDataSource = _webApplication.Services.GetRequiredService<EndpointDataSource>();
        Assert.NotNull(endpointDataSource);
        
        var routes = endpointDataSource
            .Endpoints.Select(e => e.DisplayName)
            .ToList();
        
        Assert.Equal(7, routes.Count);
        
        Assert.Contains("HTTP: GET /api/locales/ => GetLocalizationsAsync", routes);
        Assert.Contains("HTTP: PUT /api/locales/ => CreateLocalesAsync", routes);
        Assert.Contains("HTTP: GET /api/locales/{id} => GetLocalizationAsync", routes);
        Assert.Contains("HTTP: PATCH /api/locales/ => UpdateLocalesAsync", routes);
        Assert.Contains("HTTP: DELETE /api/locales/{id} => DeleteLocalesAsync", routes);
        Assert.Contains("HTTP: GET /api/locales/all-cultures => ListAllCultures", routes);
        Assert.Contains("HTTP: GET /api/locales/culture/{code} => GetCultureDetailAsync", routes);
        
       await _webApplication.StopAsync();

        
    }

    [Fact(DisplayName = "Craft Localization Module - ListAllCultures - Should return all cultures")]
    public async Task Test2()
    {
        var httpClient = _webApplication.Services.GetRequiredService<IHttpClientFactory>().CreateClient("client");
        var response = await httpClient.GetAsync("/api/locales/all-cultures");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var cultures = JsonSerializer.Deserialize<IReadOnlyList<CultureCodeAndDetailDto>>(content);
        Assert.NotNull(cultures);
        Assert.Equal(207, cultures.Count);
        httpClient.Dispose();
        await _webApplication.StopAsync();
    }
    
    [Fact(DisplayName = "Craft Localization Module - Get culture details by code - Should return culture details otherwise null")]
    public async Task Test3()
    {
        var httpClient = _webApplication.Services.GetRequiredService<IHttpClientFactory>().CreateClient("client");
        var response = await httpClient.GetAsync("/api/locales/culture/ta");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var culture = JsonSerializer.Deserialize<LocaleWithCultureDetailDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.NotNull(culture);
        Assert.Equal("ta", culture.CultureDetails.CultureCode);
        Assert.Equal("Tamil", culture.CultureDetails.Detail);
        
        
        response = await httpClient.GetAsync("/api/locales/culture/blabla");
        response.EnsureSuccessStatusCode();
        content = await response.Content.ReadAsStringAsync();
        Assert.Equal("null", content);
        
        httpClient.Dispose();

        await _webApplication.StopAsync();
    }
    
    [Fact(DisplayName = "Craft Localization Module - CreateLocalesAsync - Should create a new locale")]
    public async Task Test4()
    {
        var httpClient = _webApplication.Services.GetRequiredService<IHttpClientFactory>().CreateClient("client");
        var createLocaleDto = new CreateLocaleDto("ta", "title", "ஹலோ உள்ளூர்மயமாக்கல் தொகுதி");

        var json = JsonSerializer.Serialize(createLocaleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync("/api/locales/", content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);
        Assert.NotEmpty(responseContent);
        
        httpClient.Dispose();
        await _webApplication.StopAsync();
    }

    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
