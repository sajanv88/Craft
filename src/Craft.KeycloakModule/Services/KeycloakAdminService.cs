using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;

namespace Craft.KeycloakModule.Services;

public class KeycloakAdminService(IKeycloakClient keycloakClient)
{
    public async Task<IReadOnlyList<UserRepresentation>> GetUsers(string realm)
    {
        var results = await keycloakClient.GetUsersAsync(realm);
        return results.ToList().AsReadOnly();
    }
}
