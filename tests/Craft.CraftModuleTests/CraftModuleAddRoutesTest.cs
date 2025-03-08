using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.CraftModuleTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set the content root to the test project's directory
        builder.UseContentRoot(Directory.GetCurrentDirectory());

        // Optionally, configure services for testing
        builder.ConfigureServices(services =>
        {
            // Since we are testing the CraftModuleExtensions, we need to register the TestModule here to test the endpoint.
            // This is not necessary in the actual application.
            services.AddSingleton<TestModule>();
        });
    }
}

public sealed class CraftModuleAddRoutesTest
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CraftModuleAddRoutesTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(
        DisplayName = "AddRoutes: Should add routes to the endpoint route builder."
    )]
    public async Task Test1()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/tests");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello, World!", responseString);
    }

    [Fact(
        DisplayName = "AddRoutes: Should resolve dependencies from the service provider and returns TestModule name."
    )]
    public async Task Test2()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/tests2");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("TestModule", responseString);
    }
}
