namespace Craft.CraftModule.Dtos;

/// <summary>
///     A response object for error messages
/// </summary>
/// <param name="Error"></param>
/// <param name="Message"></param>
/// <param name="StatusCode"></param>
public record ErrorResponseDto(string Error, string Message, int StatusCode);
