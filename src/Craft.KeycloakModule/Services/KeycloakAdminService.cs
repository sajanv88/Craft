using Craft.CraftModule.Dtos;
using Craft.KeycloakModule.Options;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Craft.KeycloakModule.Services;

public class KeycloakAdminService(
    IKeycloakClient keycloakClient,
    IConfiguration configuration,
    ILogger<KeycloakAdminService> logger
)
{
    private readonly KeycloakSettings? _keycloakSettings = configuration
        .GetSection("KeycloakSettings")
        .Get<KeycloakSettings>();

    public async Task<PaginatedResponse<UserRepresentation>> GetUsers(
        int? page,
        int? limit,
        string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogInformation("Getting users via Keycloak Admin API");
        if (_keycloakSettings is null)
        {
            logger.LogError("No Keycloak Settings found");
            throw new InvalidOperationException(
                "Keycloak settings are not configured"
            );
        }
        
        var totalCount = await keycloakClient.GetUserCountAsync(
            _keycloakSettings.Realm,
            new GetUserCountRequestParameters { Search = search },
            cancellationToken
        );
        logger.LogInformation($"Total users found: {totalCount} for search: {search}");
        var requestParameters = new GetUsersRequestParameters
        {
            BriefRepresentation = true,
            First = page ?? 0,
            Max = limit ?? 10,
            Search = search,
        };
        
        var results = await keycloakClient.GetUsersAsync(
            _keycloakSettings.Realm,
            requestParameters,
            cancellationToken
        );
        logger.LogInformation($"Getting users via Keycloak Admin API completed.");
        return new PaginatedResponse<UserRepresentation>
        {
            TotalCount = totalCount,
            Items = results.ToList().AsReadOnly(),
            CurrentPage = page ?? 0,
            PageSize = limit ?? 10,
        };
    }
}
