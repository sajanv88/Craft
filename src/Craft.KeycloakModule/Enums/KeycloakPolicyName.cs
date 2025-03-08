namespace Craft.KeycloakModule.Enums;

/// <summary>
///    Defines the policies that can be assigned to users via AddPolicy.
/// </summary>
public enum KeycloakPolicyName
{
    /// <summary>
    ///    A user with the "UserPolicy" this can access the specified resource.
    /// </summary>
    UserPolicy,

    /// <summary>
    ///  A user with the "AdminPolicy" this has full access to the specified resource of the Realm in Keycloak.
    /// </summary>
    AdminPolicy,
}
