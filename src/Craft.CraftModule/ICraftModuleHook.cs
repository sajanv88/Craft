using Microsoft.Extensions.DependencyInjection;

namespace Craft.CraftModule;

/// <summary>
///
/// </summary>
public interface ICraftModuleHook
{
    /// <summary>
    ///  Hook for additional module initialization logic before it initialized.
    /// </summary>
    /// <param name="services"></param>
    void PreInitialization(IServiceCollection services);

    /// <summary>
    ///  Hook for additional module initialization logic after it initialized.
    /// </summary>
    /// <param name="services"></param>
    void PostInitialization(IServiceCollection services);
}
