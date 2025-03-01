using Craft.CraftModule.Attributes;
using Microsoft.AspNetCore.Builder;

namespace Craft.CraftModule.Extensions;

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering, initializing, and mapping Craft modules in an ASP.NET Core application.
/// </summary>
public static class CraftModuleExtensions
{
    private static HashSet<Type> InitializedModules = [];

    /// <summary>
    /// Discovers and registers all Craft modules from the specified assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the modules to.</param>
    /// <param name="assembly">The assembly to scan for Craft modules.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method scans the provided assembly for types that inherit from <see cref="CraftModule"/>,
    /// registers them in the module registry, initializes them, and adds them to the dependency injection container.
    /// </remarks>
    public static IServiceCollection AddCraftModulesFromAssembly(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        // Discover and register modules from the assembly
        var moduleTypes = assembly
            .GetTypes()
            .Where(t =>
                typeof(CraftModule).IsAssignableFrom(t) && !t.IsAbstract
            );

        foreach (var type in moduleTypes)
        {
            RegisterModule(services, type);
        }

        return services;
    }

    /// <summary>
    /// Registers the specified Craft modules.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the modules to.</param>
    /// <param name="moduleTypes">The types of the modules to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method registers the specified module types, initializes them, and adds them to the dependency injection container.
    /// </remarks>
    public static IServiceCollection AddCraftModules(
        this IServiceCollection services,
        params Type[] moduleTypes
    )
    {
        foreach (var type in moduleTypes)
        {
            RegisterModule(services, type);
        }

        return services;
    }

    /// <summary>
    /// Registers a single Craft module, initializes it, and adds it to the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the module to.</param>
    /// <param name="module">The type of the module to register.</param>
    /// <remarks>
    /// This method ensures that the module is registered in the module registry, initialized, and added to the DI container.
    /// </remarks>
    private static void RegisterModule(IServiceCollection services, Type module)
    {
        ModuleRegistry.Register(module);
        var modules = ModuleRegistry
            .GetRegisteredModules()
            .Select(type => (CraftModule)Activator.CreateInstance(type)!)
            .ToList();

        foreach (var m in modules)
        {
            InitializeModule(m, services);
            services.AddKeyedSingleton(m, module.Name); // Register module in DI
        }
    }

    /// <summary>
    /// Maps endpoints for all registered Craft modules.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to map the endpoints to.</param>
    /// <returns>The <see cref="WebApplication"/> for chaining.</returns>
    /// <remarks>
    /// This method iterates through all registered modules and calls their <see cref="CraftModule.AddRoutes"/> method
    /// to map their endpoints.
    /// </remarks>
    public static WebApplication MapCraftModulesEndpoint(
        this WebApplication app
    )
    {
        var modules = ModuleRegistry
            .GetRegisteredModules()
            .Select(type => (CraftModule)Activator.CreateInstance(type)!)
            .ToList();

        foreach (var module in modules)
        {
            module.AddRoutes(app);
        }

        return app;
    }

    /// <summary>
    /// Initializes a Craft module and ensures its dependencies are satisfied.
    /// </summary>
    /// <param name="module">The module to initialize.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a required dependency for the module is not initialized.
    /// </exception>
    /// <remarks>
    /// This method checks for <see cref="DependsOnAttribute"/> on the module and ensures all dependencies
    /// are initialized before initializing the module itself.
    /// </remarks>
    private static void InitializeModule(
        CraftModule module,
        IServiceCollection services
    )
    {
        var moduleType = module.GetType();

        if (InitializedModules.Contains(moduleType))
        {
            // Module already initialized
            return;
        }

        var dependsOnAttributes = moduleType
            .GetCustomAttributes(typeof(DependsOnAttribute), true)
            .Cast<DependsOnAttribute>();

        foreach (var attribute in dependsOnAttributes)
        {
            foreach (var dependency in attribute.Dependencies)
            {
                if (!InitializedModules.Contains(dependency))
                {
                    throw new InvalidOperationException(
                        $"Module {dependency.Name} is required for {moduleType.Name}."
                    );
                }
            }
        }
        module.PreInitialization(services);
        module.Initialize();
        InitializedModules.Add(moduleType);
        Console.WriteLine($"🚀 {moduleType.Name} initialized.");
        module.PostInitialization(services);
    }
}
