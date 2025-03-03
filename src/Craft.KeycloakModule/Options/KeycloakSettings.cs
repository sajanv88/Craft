namespace Craft.KeycloakModule.Options;

public sealed class KeycloakSettings
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    
    public bool VerifyTokenAudience { get; set; } = true;
}
