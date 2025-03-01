using Craft.CraftModule.Attributes;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.CraftModule;

/// <summary>
/// Represents a base class for Craft modules, providing functionality for initialization and route registration.
/// </summary>
/// <remarks>
/// This class serves as the foundation for all Craft modules. It enforces dependency checks using the <see cref="DependsOnAttribute"/>
/// and provides abstract methods for module initialization and route registration.
/// </remarks>
public abstract class CraftModule : ICraftModuleDatabase, ICraftModuleHook
{
    /// <summary>
    /// Initializes the module and ensures all dependencies are satisfied.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a required dependency for the module is not registered.
    /// </exception>
    /// <remarks>
    /// This method checks for <see cref="DependsOnAttribute"/> on the module and ensures all dependencies
    /// are registered before calling the <see cref="OnInitialize"/> method.
    /// </remarks>
    public void Initialize()
    {
        var dependsOnAttributes = GetType()
            .GetCustomAttributes(typeof(DependsOnAttribute), true)
            .Cast<DependsOnAttribute>();

        foreach (var attribute in dependsOnAttributes)
        {
            foreach (var dependency in attribute.Dependencies)
            {
                if (!ModuleRegistry.IsRegistered(dependency))
                {
                    throw new InvalidOperationException(
                        $"Module {dependency.Name} is required for {GetType().Name}."
                    );
                }
            }
        }
    }

    /// <summary>
    /// Adds routes for the module to the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add routes to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> for chaining.</returns>
    /// <remarks>
    /// This method is responsible for defining the routes for the module. Derived classes must implement this method
    /// to register their specific routes.
    /// </remarks>
    public abstract IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    );

    /// <summary>
    ///    Configures the main application's database context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void ConfigureModelBuilder(ModelBuilder modelBuilder)
    {
        // Default implementation does nothing
    }

    /// <summary>
    ///  Hook for additional module initialization logic before it initialized.
    /// </summary>
    /// <param name="services"></param>
    public virtual void PreInitialization(IServiceCollection services)
    {
        // Default implementation does nothing
    }

    /// <summary>
    ///  Hook for additional module initialization logic after it initialized.
    /// </summary>
    /// <param name="services"></param>
    public virtual void PostInitialization(IServiceCollection services)
    {
        // Default implementation does nothing
    }
}
