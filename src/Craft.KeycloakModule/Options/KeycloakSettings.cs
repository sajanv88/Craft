namespace Craft.KeycloakModule.Options;

public sealed class Admin
{
    /// <summary>
    ///    The client ID of the Keycloak client that has the realm-management role.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    /// <summary>
    ///    The client secret of the Keycloak client that has the realm-management role.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
///     Contains the settings required to configure Keycloak-based authentication and authorization in an ASP.NET Core application.
/// </summary>
public sealed class KeycloakSettings
{
    /// <summary>
    ///     A URL that points to the Keycloak server's realm example: https://keycloak.local/realms/dev
    /// </summary>
    public string Authority { get; set; } = string.Empty;
    /// <summary>
    ///     A URL that points to the Keycloak server's realm example: https://keycloak.local/realms/dev
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
    /// <summary>
    ///     A URL that points to the Keycloak server for example: https://keycloak.local
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    /// <summary>
    ///    A Realm is a container for a set of users, credentials, roles, and groups. A user belongs to and logs into a realm.
    /// </summary>
    public string Realm { get; set; } = string.Empty;
    
    /// <summary>
    ///     Audience is the intended recipient of the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     Verify the token audience.
    /// </summary>
    public bool VerifyTokenAudience { get; set; } = true;

    /// <summary>
    ///     Metadata address for the OpenID Connect discovery document. For example, https://keycloak.local/realms/dev/.well-known/openid-configuration
    /// </summary>
    public string MetadataAddress { get; set; } = string.Empty;

    /// <summary>
    ///     Admin settings for the Keycloak client that has the realm-management role. Mostly used via Service Account. Read more on keycloak.org
    /// </summary>
    public Admin? Admin { get; set; } = null;
}
