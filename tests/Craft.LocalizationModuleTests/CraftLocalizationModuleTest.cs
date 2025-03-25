using System.Net.Http.Json;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Dtos;
using Craft.CraftModule.Extensions;
using Craft.LocalizationModule.Dtos;
using Craft.LocalizationModule.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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

public class CraftLocalizationModuleTest : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    private TestDatabaseFixture _fixture;
    private WebApplicationBuilder _webApplicationBuilder;
    private WebApplication _webApplication;
    private EndpointDataSource _endpointDataSource;
    private IServiceScope _scope;
    private HttpClient _httpClient;

    public CraftLocalizationModuleTest(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitializeAsync();
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", _fixture.ConnectionString }
        };
        Console.WriteLine($"Using database connection string: {_fixture.ConnectionString}");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _webApplicationBuilder = WebApplication.CreateBuilder();


        var services = _webApplicationBuilder.Services;
        services.AddHttpClient("client", client => { client.BaseAddress = new Uri("http://localhost:5000"); });
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddCraftLocalization(option => { option.SupportedCultureCodes = []; });

        CraftModuleExtensions.InitializedModules.Clear();
        services.AddCraftModules([typeof(LocalizationModuleTest)]);

        _webApplication = _webApplicationBuilder.Build();
        _webApplication.UseCraftGeneralException();

        _webApplication.MapCraftModulesEndpoint();

        await _webApplication.StartAsync();
        _scope = _webApplication.Services.CreateScope();
        _endpointDataSource = _scope.ServiceProvider.GetRequiredService<EndpointDataSource>();
        _httpClient = _scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("client");
    }

    [Fact(DisplayName = "Craft Localization Module default endpoints are registered.")]
    public void Test1()
    {
        var routes = _endpointDataSource
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
    }

    [Fact(DisplayName = "Craft Localization Module - ListAllCultures - Should return all cultures")]
    public async Task Test2()
    {
        var cultures =
            await _httpClient.GetFromJsonAsync<IReadOnlyList<CultureCodeAndDetailDto>>("/api/locales/all-cultures");
        Assert.NotNull(cultures);
        Assert.Equal(207, cultures.Count);
    }

    [Fact(DisplayName = "Craft Localization Module - CreateLocalesAsync - Should create a new locale")]
    public async Task Test3()
    {
        var createLocaleDto = new CreateLocaleDto("ta", "title", "ஹலோ உள்ளூர்மயமாக்கல் தொகுதி");

        var response = await _httpClient.PutAsJsonAsync("/api/locales/", createLocaleDto);
        response.EnsureSuccessStatusCode();
        var localId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.IsType<Guid>(localId);
    }

    [Fact(DisplayName =
        "Craft Localization Module - Get culture details by code - Should return culture details otherwise null")]
    public async Task Test4()
    {
        var culture = await _httpClient.GetFromJsonAsync<LocaleWithCultureDetailDto>("/api/locales/culture/ta");
        Assert.NotNull(culture);
        Assert.Equal("ta", culture.CultureDetails.CultureCode);
        Assert.Equal("Tamil", culture.CultureDetails.Detail);

        culture = await _httpClient.GetFromJsonAsync<LocaleWithCultureDetailDto>("/api/locales/culture/blabla");
        Assert.Null(culture);
    }

    [Fact(DisplayName = "Craft Localization Module - GetLocalizationAsync - Should return a locale by id")]
    public async Task Test5()
    {
        var createLocaleDto = new CreateLocaleDto("en-US", "title", "Hello");
        var response = await _httpClient.PutAsJsonAsync("/api/locales/", createLocaleDto);
        response.EnsureSuccessStatusCode();
        var guid = await response.Content.ReadFromJsonAsync<Guid>();

        var locale = await _httpClient.GetFromJsonAsync<LocaleDto>($"/api/locales/{guid}");
        Assert.NotNull(locale);
        Assert.Equal("en-US", locale.CultureCode);
        Assert.Equal("title", locale.Key);
        Assert.Equal("Hello", locale.Value);
    }

    [Fact(DisplayName =
        "Craft Localization Module - CreateLocalesAsync - Should throw a bad request exception when passed invalid data")]
    public async Task Test6()
    {
        var createLocaleDto = new CreateLocaleDto("unknown-culture-code", "title", "ஹலோ உள்ளூர்மயமாக்கல் தொகுதி");

        var response = await _httpClient.PutAsJsonAsync("/api/locales/", createLocaleDto);
        Assert.False(response.IsSuccessStatusCode);
        var errorResponseDto = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        Assert.NotNull(errorResponseDto);
        Assert.Equal(400, errorResponseDto.StatusCode);

        createLocaleDto = new CreateLocaleDto("en-US", "", "hello world");

        response = await _httpClient.PutAsJsonAsync("/api/locales/", createLocaleDto);
        Assert.False(response.IsSuccessStatusCode);
        errorResponseDto = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        Assert.NotNull(errorResponseDto);
        Assert.Equal(400, errorResponseDto.StatusCode);
    }

    [Fact(DisplayName = "Craft Localization Module - UpdateLocalesAsync - Should update a locale")]
    public async Task Test7()
    {
        var createLocaleDto = new CreateLocaleDto("en-US", "nav_menu", "Home");
        var response = await _httpClient.PutAsJsonAsync("/api/locales", createLocaleDto);
        response.EnsureSuccessStatusCode();
        var guid = await response.Content.ReadFromJsonAsync<Guid>();
        var updateLocaleDto = new UpdateLocaleDto(guid, "en-US", "nav_menu", "About");
        response = await _httpClient.PatchAsJsonAsync("/api/locales", updateLocaleDto);
        response.EnsureSuccessStatusCode();
        var locale = await response.Content.ReadFromJsonAsync<LocaleDto>();
        Assert.NotNull(locale);
        Assert.Equal("en-US", locale.CultureCode);
        Assert.Equal("nav_menu", locale.Key);
        Assert.Equal("About", locale.Value);
    }

    [Fact(DisplayName = "Craft Localization Module - UpdateLocalesAsync - Should throw a bad request exception when passed invalid data")]
    public async Task Test8()
    {
        var createLocaleDto = new CreateLocaleDto("en-US", "header", "Header1");
        var response = await _httpClient.PutAsJsonAsync("/api/locales", createLocaleDto);
        response.EnsureSuccessStatusCode();
        var guid = await response.Content.ReadFromJsonAsync<Guid>();
        var updateLocaleDto = new UpdateLocaleDto(guid, "en-US", "", "About");
        response = await _httpClient.PatchAsJsonAsync("/api/locales", updateLocaleDto);
        Assert.False(response.IsSuccessStatusCode);
        var errorResponseDto = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        Assert.NotNull(errorResponseDto);
        Assert.Equal(400, errorResponseDto.StatusCode);
    }

    [Fact(DisplayName = "Craft Localization Module - DeleteLocalesAsync - Should delete a locale")]
    public async Task Test9()
    {
        var createLocaleDto = new CreateLocaleDto("en-US", "page_title", "MainPage");
        var response = await _httpClient.PutAsJsonAsync("/api/locales", createLocaleDto);
        response.EnsureSuccessStatusCode();
        var guid = await response.Content.ReadFromJsonAsync<Guid>();
        response = await _httpClient.DeleteAsync($"/api/locales/{guid}");
        response.EnsureSuccessStatusCode();
        
        response = await _httpClient.GetAsync($"/api/locales/{guid}");
        var locale = await response.Content.ReadFromJsonAsync<LocaleDto>();
        Assert.Null(locale);
    }
    
    public async Task DisposeAsync()
    {
        _webApplication?.DisposeAsync();
    }
}