using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Craft.KeycloakModule.Extensions;
// opt.Authority = keycloakSettings.Authority;
// opt.Audience = keycloakSettings.Audience;
// opt.MetadataAddress = keycloakSettings.MetadataAddress;
//                
// opt.TokenValidationParameters = new TokenValidationParameters
// {
//     ValidIssuer = keycloakSettings.Authority,
//     ValidateIssuer = true,
//     ValidateAudience = true,
//     ValidateLifetime = true,
//     ValidateIssuerSigningKey = true,
// };
//                 
// opt.Events = new JwtBearerEvents
// {
//     OnAuthenticationFailed = c =>
//     {
//         Console.WriteLine($"🔴 Authentication failed: {c.Exception.Message}");
//         return Task.CompletedTask;
//     },
//     OnTokenValidated = c =>
//     {
//         Console.WriteLine("✅ Token successfully validated!");
//         return Task.CompletedTask;
//     }
// };
public static class CraftKeycloakExtensions
{
    public static IServiceCollection AddCraftKeycloakAuthorization(this IServiceCollection services,
        Action<KeycloakAuthorizationOptions>? configureKeycloakAuthorizationOptions = null)
    {
        services.AddKeycloakAuthorization(configureKeycloakAuthorizationOptions);
        return services;
    }
    
    public static IServiceCollection AddCraftKeycloakAuthentication(this IServiceCollection services,
        Action<KeycloakAuthenticationOptions> keycloakAuthenticationOptions,
        Action<JwtBearerOptions>? configureJwtBearerOptions = null)
    {
        services.AddKeycloakWebApiAuthentication(keycloakAuthenticationOptions, configureJwtBearerOptions);
        return services;
    }
}
