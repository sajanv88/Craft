# Craft.KeycloakModule

The Keycloak Module for Craft simplifies the integration of Keycloak-based authentication and 
authorization into your application. By default, Craft leverages Keycloak's 
robust identity and access management services to secure your application, ensuring a 
seamless and secure user experience.

## Getting Started
Craft's Keycloak Module is designed to be easy to use and integrate into your application. Hence, 
it is dependent on this following libraries:

- `Keycloak.AuthServices.Authentication`
- `Keycloak.AuthServices.Authorization`
- `Keycloak.AuthServices.Sdk`
- `Keycloak.AuthServices.Sdk.Kiota`
- `Duende.AccessTokenManagement`

You can read more about these libraries on their respective GitHub repositories.
- [Keycloak.AuthServices.Authentication](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)
- [Keycloak.AuthServices.Authorization](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)
- [Keycloak.AuthServices.Sdk](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)
- [Keycloak.AuthServices.Sdk.Kiota](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)
- [Duende.AccessTokenManagement](https://nikiforovall.github.io/keycloak-authorization-services-dotnet/admin-rest-api/access-token.html)

### To Get Started
1. Install the `dotnet add package Craft.KeycloakModule` NuGet package in your existing Craft API project.
2. Create a keycloakSettings in your `appsettings.json` file.

```json
{
    "KeycloakSettings": {
    "Authority": "https://keycloak.local/realms/dev",
    "Audience": "account",
    "ClientId": "craft-api",
    "BaseUrl": "https://keycloak.local",
    "Realm": "dev",
    "VerifyTokenAudience": true,
    "MetadataAddress": "https://keycloak.local/realms/dev/.well-known/openid-configuration",
    "Admin": {
      "ClientId": "admin-client",
      "ClientSecret": "dcOwlcsgCMSssAh4rjoWW99UWWPNq6Qq"
    }
  }
 }
 ```

3. Add the following code to your `Program.cs` file:

```csharp

var keycloakSettings = builder
    .Configuration.GetSection("KeycloakSettings")
    .Get<KeycloakSettings>();

builder.Services.AddCraftKeycloakAuthorization(options =>
{
    options.Realm = keycloakSettings.Realm;
    options.Resource = keycloakSettings.ClientId;
});

builder.Services.AddCraftKeycloakAuthentication(
    options =>
    {
        options.Realm = keycloakSettings.Realm;
        options.Audience = keycloakSettings.Audience;
        options.AuthServerUrl = keycloakSettings.BaseUrl;
    },
    options =>
    {
        options.MetadataAddress = keycloakSettings.MetadataAddress;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = keycloakSettings.Authority,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = c =>
            {
                Console.WriteLine(
                    $"🔴 Authentication failed: {c.Exception.Message}"
                );
                return Task.CompletedTask;
            },
            OnTokenValidated = c =>
            {
                Console.WriteLine("✅ Token successfully validated!");
                return Task.CompletedTask;
            },
        };
    }
);

```


`AddCraftKeycloakAuthorization ` is a wrapper of `AddKeycloakAuthorization` from `Keycloak.AuthServices.Authorization` library.
all the options are the same. You can read more about it [here](https://nikiforovall.github.io/keycloak-authorization-services-dotnet/authorization/authorization-server.html)

`AddCraftKeycloakAuthentication` is a wrapper of `AddKeycloakWebApiAuthentication` from `Keycloak.AuthServices.Authentication` library.
all the options are the same. You can read more about it [here](https://nikiforovall.github.io/keycloak-authorization-services-dotnet/configuration/configuration-authentication.html)


3. Mark any of your exising Craft modules with the `[DependsOn]` attribute to secure the module.

```csharp
[DependsOn(typeof(KeycloakModule))]
public sealed class ApiModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(
        IEndpointRouteBuilder builder
    )
    {
        var app = builder.MapGroup("/api").RequireAuthorization(nameof(KeycloakPolicyName.AdminPolicy));
        app.MapGet("/", () => "Hello from ApiModule!");
        return builder;
    }
}
```

This will secure the `ApiModule` with Keycloak authentication and authorization. By default, the module will be protected with the `Bearer` token. 
Also, it comes with default `User` and `Admin` roles. You can simply create a new role in your Keycloak admin console and assign it to your users.
Read keycloak documentation for more information.


4. Run your application and navigate to the `/api/keycloak/*` endpoint. You will be redirected to the Keycloak login page.

```text
    HTTP: GET /api/keycloak/admin/users
          GET /api/keycloak/admin/roles
          GET /api/keycloak/admin/clients
          GET /api/keycloak/profile/me
```

For better understanding, please run the `Craft.Api` project inside this repository. It has a demonstration of Craft extensions around a common ASP.NET Core Web API project.
Comes with Scalar playground and a simple todo API example.

### Additional Information
- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [Keycloak Admin API](https://www.keycloak.org/docs-api/12.0/rest-api/index.html)
- [Keycloak-authorization-services](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)


## Coming soon
- [ ] Keycloak Admin User management UI
- [ ] More tutorials and examples

