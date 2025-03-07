namespace Craft.KeycloakModule.Enums;

/// <summary>
///    Defines the roles that can be assigned to users in Keycloak.
/// </summary>
public enum KeycloakRoles
{
    /// <summary>
    ///  A user with the "Admin" role has full access to the specified resource Realm in the keycloak.
    /// </summary>
    Admin,
    /// <summary>
    ///    A user with the "User" role has limited access.
    /// </summary>
    User,
}
