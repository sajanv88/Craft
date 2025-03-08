using Craft.CraftModule.Exceptions;
using Microsoft.AspNetCore.Builder;

namespace Craft.CraftModule.Extensions;

/// <summary>
///
/// </summary>
public static class CraftModuleMiddlewareExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCraftGeneralException(
        this IApplicationBuilder app
    )
    {
        return app.UseMiddleware<CraftGeneralException>();
    }
}
