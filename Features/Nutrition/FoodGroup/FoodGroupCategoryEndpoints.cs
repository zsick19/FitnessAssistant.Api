using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FitnessAssistant.Api.Features.Nutrition.Category;

public static class FoodGroupCategory
{
    private const string GetFoodGroup = nameof(GetFoodGroup);
    public static void MapFoodGroupEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (FitnessAssistantContext dbContext) =>
        {
            var foundFoodGroups = await dbContext.FoodGroups.ToListAsync();
            return new FoodGroupPageResponseDto(foundFoodGroups);
        }).AllowAnonymous();

        app.MapGet("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            FoodGroup? foundFoodGroup = await dbContext.FoodGroups.FindAsync(id);
            return foundFoodGroup is null ? Results.NotFound() : Results.Ok(foundFoodGroup);
        }).WithName(GetFoodGroup).AllowAnonymous();

        app.MapPost("/", async (FitnessAssistantContext dbContext, CreateFoodGroupDto createFoodGroupDto, ClaimsPrincipal userClaim) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }

            FoodGroup createdFoodGroup = new FoodGroup()
            {
                Name = createFoodGroupDto.Name
            };

            dbContext.FoodGroups.Add(createdFoodGroup);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetFoodGroup, new { id = createdFoodGroup.Id }, createdFoodGroup);

        }).AllowAnonymous();

        app.MapPut("/{id}", async (FitnessAssistantContext dbContext, Guid id, UpdateFoodGroupDto updateFoodGroupDto, ClaimsPrincipal userClaim) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }

            var existingFoodGroup = await dbContext.FoodGroups.FindAsync(id);
            if (existingFoodGroup is null) { return Results.NotFound(); }

            existingFoodGroup.Name = updateFoodGroupDto.Name;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).WithParameterValidation().RequireAuthorization(Policies.AdminAccess);

        app.MapDelete("/{id}", async (FitnessAssistantContext dbContext, Guid id) =>
        {
            await dbContext.FoodGroups.Where(foodGroup => foodGroup.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization(Policies.AdminAccess);
    }
}
