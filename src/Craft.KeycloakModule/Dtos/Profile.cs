namespace Craft.KeycloakModule.Dtos;

/// <summary>
///  Represents a user profile.
/// </summary>
public sealed class Profile
{
    /// <summary>
    ///     Sub is the unique identifier for the user.
    /// </summary>
    public string Sub { get; set; } = string.Empty;
    /// <summary>
    ///     Email is the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    ///  FullName is the full name of the user.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    ///     PreferredUsername is the preferred username of the user.
    /// </summary>
    public string PreferredUsername { get; set; } = string.Empty;
    /// <summary>
    ///     Boolean value indicating if the email is verified.
    /// </summary>
    public bool IsEmailVerified { get; set; } = false;
}
