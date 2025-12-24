using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FitnessAssistant.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    private const string ApiAccessScopeAll = "fitnessAssistant_api.all";

    public static IHostApplicationBuilder AddFitnessAssistantAuthentication(this IHostApplicationBuilder builder)
    {

        builder.Services.AddSingleton<KeyCloakClaimsTransformer>();
        builder.Services.AddAuthentication(Schemes.KeyCloak)
        .AddJwtBearer(
            options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
            })
            .AddJwtBearer(Schemes.KeyCloak,
            options =>
            {
                options.Authority = "http://localhost:8080/realms/FitnessAssistant";
                options.Audience = "fitnessAssistant-backendApi";
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                options.RequireHttpsMetadata = false;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var transformer = context.HttpContext.RequestServices.GetRequiredService<KeyCloakClaimsTransformer>();
                        transformer.Transform(context);
                        return Task.CompletedTask;
                    }
                };
            });
        return builder;
    }

    public static IHostApplicationBuilder AddFitnessAssistantAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
                        .AddFallbackPolicy(Policies.UserAccess, authBuilder =>
                        {
                            authBuilder.RequireClaim(ClaimTypes.Scope, ApiAccessScopeAll);
                        })
                        .AddPolicy(Policies.AdminAccess, authBuilder =>
                        {
                            authBuilder.RequireClaim(ClaimTypes.Scope, ApiAccessScopeAll);
                            authBuilder.RequireRole(Roles.Admin);
                        });
        return builder;
    }
}
