namespace Craft.KeycloakModule.Dtos;

public sealed class Profile
{
    public string Sub { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PreferredUsername { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
}
