using Craft.CraftModule.Dtos;
using Craft.KeycloakModule.Options;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Keycloak.AuthServices.Sdk.Kiota.Admin;
using Keycloak.AuthServices.Sdk.Kiota.Admin.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Craft.KeycloakModule.Services;

/// <summary>
///     A service that provides methods for interacting with the Keycloak Admin API.
/// </summary>
/// <param name="keycloakAdminApiClient"></param>
/// <param name="configuration"></param>
/// <param name="logger"></param>
public sealed class KeycloakAdminService(
    KeycloakAdminApiClient keycloakAdminApiClient,
    IConfiguration configuration,
    ILogger<KeycloakAdminService> logger
)
{
    private readonly KeycloakSettings? _keycloakSettings = configuration
        .GetSection("KeycloakSettings")
        .Get<KeycloakSettings>();

    private void ValidateKeycloakSettings()
    {
        if (_keycloakSettings is not null)
            return;
        logger.LogError("No Keycloak Settings found");
        throw new InvalidOperationException(
            "Keycloak settings are not configured"
        );
    }

    /// <summary>
    ///     Retrieves a list of users from the Keycloak Admin API.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <param name="search"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PaginatedResponse<UserRepresentation>> GetUsers(
        int? page,
        int? limit,
        string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Getting users via Keycloak Admin API");
        ValidateKeycloakSettings();
        var users = keycloakAdminApiClient
            .Admin
            .Realms[_keycloakSettings.Realm]
            .Users;

        var requestParameters = new GetUsersRequestParameters
        {
            First = page ?? 0,
            Max = limit ?? 10,
            Search = search,
        };
        var totalCount = await users.Count.GetAsync(
            p =>
            {
                p.QueryParameters.Email = requestParameters.Search;
                p.QueryParameters.Search = requestParameters.Search;
            },
            cancellationToken
        );
        logger.LogInformation(
            $"Total users found: {totalCount} for search: {search}"
        );

        var results = await users.GetAsync(
            p =>
            {
                p.QueryParameters.Email = requestParameters.Search;
                p.QueryParameters.First = requestParameters.First;
                p.QueryParameters.Max = requestParameters.Max;
                p.QueryParameters.Search = requestParameters.Search;
            },
            cancellationToken
        );
        logger.LogInformation(
            $"Getting users via Keycloak Admin API completed."
        );
        return new PaginatedResponse<UserRepresentation>
        {
            TotalCount = totalCount ?? 0,
            Items = results.AsReadOnly(),
            CurrentPage = page ?? 0,
            PageSize = limit ?? 10,
        };
    }

    /// <summary>
    ///     Retrieves a list of roles from the Keycloak Admin API.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <param name="search"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PaginatedResponse<RoleRepresentation>> GetRoles(
        int? page,
        int? limit,
        string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        ValidateKeycloakSettings();
        var roles = keycloakAdminApiClient
            .Admin
            .Realms[_keycloakSettings.Realm]
            .Roles;
        var results = await roles.GetAsync(
            o =>
            {
                o.QueryParameters.First = page ?? 0;
                o.QueryParameters.Max = limit ?? 10;
                o.QueryParameters.Search = search;
            },
            cancellationToken
        );
        return new PaginatedResponse<RoleRepresentation>
        {
            Items = results.AsReadOnly(),
            CurrentPage = page ?? 0,
            PageSize = limit ?? 10,
        };
    }

    /// <summary>
    ///     Retrieves a list of clients from the Keycloak Admin API.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PaginatedResponse<ClientRepresentation>> GetClients(
        int? page,
        int? limit,
        CancellationToken cancellationToken = default
    )
    {
        ValidateKeycloakSettings();
        var clients = keycloakAdminApiClient
            .Admin
            .Realms[_keycloakSettings.Realm]
            .Clients;

        var results = await clients.GetAsync(
            o =>
            {
                o.QueryParameters.First = page ?? 0;
                o.QueryParameters.Max = limit ?? 10;
                o.QueryParameters.Search = true;
            },
            cancellationToken
        );
        return new PaginatedResponse<ClientRepresentation>
        {
            Items = results.AsReadOnly(),
            CurrentPage = page ?? 0,
            PageSize = limit ?? 10,
        };
    }
}
