namespace Craft.KeycloakModule.Enums;

/// <summary>
///    Defines the policies that can be assigned to users via AddPolicy.
/// </summary>
public enum KeycloakPolicyName
{
    /// <summary>
    ///    A user with the "User" policy can access the specified resource.
    /// </summary>
    User,
    /// <summary>
    ///  A user with the "Admin" policy has full access to the specified resource of the Realm in Keycloak.
    /// </summary>
    Admin,
}
