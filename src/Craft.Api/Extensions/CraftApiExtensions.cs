using Craft.KeycloakModule.Options;
using Microsoft.OpenApi.Models;

namespace Craft.Api.Extensions;

public static class CraftApiExtensions
{
    public static IServiceCollection AddOpenApiOauth(this IServiceCollection services, IConfiguration configuration)
    {
 
            var keycloakSetting = configuration.GetSection("KeycloakSettings").Get<KeycloakSettings>();
            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "OAuth2",
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer",
                    },
                    ["openid", "profile", "email", "api", "offline_access"]
                },
            };
        
            var securityDefinitions = new Dictionary<string, OpenApiSecurityScheme>
            {
                {
                    "OAuth2",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(
                                    $"{keycloakSetting.Authority}/protocol/openid-connect/auth"
                                ),
                                TokenUrl = new Uri(
                                    $"{keycloakSetting.Authority}/protocol/openid-connect/token"
                                ),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "OpenID Connect" },
                                    { "profile", "Access profile information" },
                                    { "email", "Access email information" },
                                    { "offline_access", "Offline access" },
                                },
                            },
                        },
                    }
                },
            };
            
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi(options =>
            {
                options.AddOperationTransformer(
                    (operation, context, arg3) =>
                    {
                        operation.Security = new List<OpenApiSecurityRequirement>
                        {
                            securityRequirement,
                        };
                        return Task.CompletedTask;
                    }
                );

                options.AddDocumentTransformer(
                    (document, context, arg3) =>
                    {
                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes = securityDefinitions;
                        return Task.CompletedTask;
                    }
                );
            });
        return services;
    }
}
