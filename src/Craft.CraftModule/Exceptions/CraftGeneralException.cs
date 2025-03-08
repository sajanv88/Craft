using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions;

namespace Craft.CraftModule.Exceptions;

/// <summary>
///
/// </summary>
public class CraftGeneralException
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CraftGeneralException> _logger;

    /// <summary>
    ///
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public CraftGeneralException(
        RequestDelegate next,
        ILogger<CraftGeneralException> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception
    )
    {
        
        context.Response.ContentType = "application/json";
        var statusCode = exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            ApiException => StatusCodes.Status401Unauthorized,
            
            _ => StatusCodes.Status500InternalServerError,
        };
        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = "An error occurred while processing your request. See details for more information.",
            details = exception.Message,
            statusCode = statusCode,
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
