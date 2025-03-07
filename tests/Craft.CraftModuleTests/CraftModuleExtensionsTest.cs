using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.CraftModuleTests;

public sealed class TestModule : CraftModule.CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var endpoints = builder.MapGroup("/tests");
        endpoints.MapGet(
            "/",
            () => Results.Text("Hello, World!", "text/plain")
        );
        return builder;
    }

    public string GetModuleName() => nameof(TestModule);
}

public sealed class TestModule2 : CraftModule.CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var testModule =
            builder.ServiceProvider.GetRequiredService<TestModule>();
        var endpoints = builder.MapGroup("/tests2");
        endpoints.MapGet(
            "/",
            () => Results.Text(testModule.GetModuleName(), "text/plain")
        );
        return builder;
    }

    public string GetModuleName() => nameof(TestModule2);
}

public sealed class TestModule3
{
    public string GetModuleName() => nameof(TestModule3);
}


public sealed class TestModule4Service
{
    public string GetName() => nameof(TestModule4Service);
}
public sealed class TestModuleBase4 : CraftModule.CraftModule
{
    public override void PreInitialization(IServiceCollection services)
    {
        services.AddSingleton<TestModule4Service>();
    }

    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        return builder;
    }
    
    public string GetModuleName() => nameof(TestModuleBase4);
}

[DependsOn(typeof(TestModuleBase4))]
public sealed class TestModule4 : CraftModule.CraftModule 
{
    private readonly TestModule4Service _service;
    public TestModule4() {}
    public TestModule4(TestModule4Service service)
    {
        this._service = service;
    }
    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        return builder;
    }
    
    public string GetModuleName() => $"{nameof(TestModule4)} {_service.GetName()}";
}


public class CraftModuleExtensionsTest
{
    [Fact(
        DisplayName = "AddCraftModulesFromAssembly: Extension method should register all Craft modules from the specified assembly. And register in the service collections."
    )]
    public void Test1()
    {
        var services = new ServiceCollection();
        services.AddCraftModulesFromAssembly(
            typeof(CraftModuleExtensionsTest).Assembly
        );
        Assert.True(CraftModuleExtensions.InitializedModules.Count > 0);
        foreach (var m in CraftModuleExtensions.InitializedModules)
        {
            Assert.NotNull(m);
            Assert.True(m.IsSubclassOf(typeof(CraftModule.CraftModule)));
        }
        var serviceProvider = services.BuildServiceProvider();
        var testModule = serviceProvider.GetRequiredService<TestModule>();
        Assert.NotNull(testModule);

        Assert.Equal("TestModule", testModule.GetModuleName());

        Assert.Throws<InvalidOperationException>(
            () => serviceProvider.GetRequiredService<TestModule3>()
        );
    }

    [Fact(
        DisplayName = "AddCraftModules: Registers the specified module types, initializes them, and adds them to the dependency injection container."
    )]
    public void Test2()
    {
        var services = new ServiceCollection();
        services.AddCraftModules([typeof(TestModule2)]);

        var serviceProvider = services.BuildServiceProvider();
        var testModule2 = serviceProvider.GetRequiredService<TestModule2>();
        Assert.NotNull(testModule2);

        Assert.Equal("TestModule2", testModule2.GetModuleName());

        Assert.Throws<InvalidOperationException>(
            () => serviceProvider.GetRequiredService<TestModule3>()
        );
    }


    [Fact(
        DisplayName =
            "AddCraftModules: Registers the specified module types which has a constructor, initializes them, and adds them to the dependency injection container."
    )]
    public void Test3()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TestModule4Service>(); // Register the service for testing

        services.AddCraftModules([typeof(TestModule4)]);
        
        var serviceProvider = services.BuildServiceProvider();
        var ex = Record.Exception(() => serviceProvider.GetRequiredService<TestModule4>());
        Assert.Null(ex);
        
        var testModule4 = serviceProvider.GetRequiredService<TestModule4>();
        var output = testModule4.GetModuleName();
        Assert.Equal("TestModule4 TestModule4Service", output);
    }
}
