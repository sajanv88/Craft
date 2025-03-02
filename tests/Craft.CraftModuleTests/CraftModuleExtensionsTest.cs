using Craft.CraftModule.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.CraftModuleTests;

public sealed class TestModule : CraftModule.CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("/tests");
        endpoints.MapGet("/", () => Results.Ok("Hello, World!"));
        return builder;
    }
}
public class CraftModuleExtensionsTest
{
    [Fact(DisplayName = "AddCraftModulesFromAssembly: Extension method should register all Craft modules from the specified assembly.")]
    public void Test1()
    {
        var services = new ServiceCollection();
        services.AddCraftModulesFromAssembly(typeof(CraftModuleExtensionsTest).Assembly);
        Assert.True(CraftModuleExtensions.InitializedModules.Count > 0);
        foreach (var m in CraftModuleExtensions.InitializedModules)
        {
            Assert.NotNull(m);
            Assert.True(m.IsSubclassOf(typeof(CraftModule.CraftModule)));
            Assert.Equal("TestModule", m.Name);
        }
        
    }
    
    [Fact(DisplayName = "AddCraftModulesFromAssembly: Register in the service collections.")]
    public void Test2()
    {
        var services = new ServiceCollection();
        services.AddCraftModulesFromAssembly(typeof(CraftModuleExtensionsTest).Assembly);
    
        var serviceProvider = services.BuildServiceProvider();
        var module = serviceProvider.GetRequiredService<TestModule>();
        Assert.NotNull(module);
    }
}
