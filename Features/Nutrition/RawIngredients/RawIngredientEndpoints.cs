using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Features.Users.Constants;
using FitnessAssistant.Api.Shared.Authorization;
using FitnessAssistant.Api.Shared.FileUpload;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitnessAssistant.Api.Features.Nutrition.RawIngredients;

public static class RawIngredientEndpoints
{
    private const string DefaultRawImageProfileUri = "https://placehold.co/100";

    private const string GetRawIngredient = nameof(GetRawIngredient);
    public static void MapRawIngredientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (FitnessAssistantContext dbContext, [AsParameters] GetRawIngredientsRequestDto queryRequest) =>
        {
            var skipCount = (queryRequest.pageNumber - 1) * queryRequest.pageSize;

            var filteredRawIngredients = dbContext.RawIngredients.AsQueryable();
            if (queryRequest.searchCategory != null)
            {
                filteredRawIngredients = filteredRawIngredients.Where(rawIngredient => rawIngredient.FoodGroupId == new Guid(queryRequest.searchCategory));
            }
            if (queryRequest.searchName != null)
            {
                filteredRawIngredients = filteredRawIngredients.Where(ingredient => string.IsNullOrWhiteSpace(queryRequest.searchName) || ingredient.Name.ToLower().Contains(queryRequest.searchName.ToLower()) || EF.Functions.Like(ingredient.Name, $"%{queryRequest.searchName}%"));
            }

            var ingredientsOnPage = await filteredRawIngredients.OrderBy(ingredient => ingredient.Name)
                .Skip(skipCount)
                .Take(queryRequest.pageSize)
                .Include(ingredient => ingredient.FoodGroup)
                    .Select(ingredient => new GetRawIngredientSummaryResponseDto(ingredient.Id, ingredient.Name, ingredient.CaloriesPerUnit, ingredient.UnitOfMeasurement, ingredient.BaseLineMeasurement, ingredient.FoodGroup!.Name, ingredient.ProteinPerUnit))
                    .AsNoTracking()
                    .ToListAsync();

            var totalRawIngredients = await filteredRawIngredients.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRawIngredients / (double)queryRequest.pageSize);

            return new RawIngredientsSummaryPageResponseDto(totalPages, ingredientsOnPage);
        }).AllowAnonymous();

        app.MapGet("/{ingredientId}", async (FitnessAssistantContext dbContext, Guid ingredientId) =>
        {
            RawIngredient? foundRawIngredient = await dbContext.RawIngredients
                                                                .Include(ingredient => ingredient.FoodGroup)
                                                                .FirstOrDefaultAsync(o => o.Id == ingredientId);


            return foundRawIngredient is null ? Results.NotFound() : Results.Ok(
                new RawIngredientDetailedResponseDto(
                    foundRawIngredient.Id,
                    foundRawIngredient.Name,
                    foundRawIngredient.CaloriesPerUnit,
                    foundRawIngredient.ProteinPerUnit,
                    foundRawIngredient.UnitOfMeasurement,
                    foundRawIngredient.BaseLineMeasurement,
                    foundRawIngredient.RecipeCount,
                    foundRawIngredient.FoodGroup!.Name,
                    foundRawIngredient.ProfileImageUri
                )
            );
        }).WithName(GetRawIngredient).AllowAnonymous();

        app.MapPost("/", async (FitnessAssistantContext dbContext, [FromForm] CreateRawIngredientReqDto createRawIngredientReq, ILoggerFactory loggerFactory, FileUploader fileUploader, ClaimsPrincipal userClaim) =>
        {
            // if (!userClaim?.Identity?.IsAuthenticated == true) return Results.Unauthorized();

            // var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ??
            //                                 userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            // if (string.IsNullOrEmpty(currentUserId)) return Results.Unauthorized();

            var imageUri = DefaultRawImageProfileUri;
            if (createRawIngredientReq.ImageFile is not null)
            {
                var fileUploadResult = await fileUploader.UploadFileAsync(createRawIngredientReq.ImageFile, StorageNames.RawIngredientImageFolder);
                if (!fileUploadResult.IsSuccess)
                {
                    return Results.BadRequest(new { message = fileUploadResult.ErrorMessage });
                }
                imageUri = fileUploadResult.FileUrl;
            }

            RawIngredient createdRawIngredient = new RawIngredient()
            {
                Name = createRawIngredientReq.Name,
                CaloriesPerUnit = createRawIngredientReq.Calories,
                ProteinPerUnit = createRawIngredientReq.Protein,
                UnitOfMeasurement = createRawIngredientReq.UnitOfMeasurement,
                BaseLineMeasurement = createRawIngredientReq.BaseLineMeasurement,
                FoodGroupId = createRawIngredientReq.FoodGroup,
                ProfileImageUri = imageUri!,
                SubmittedById = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348"),
                LastUpdatedBy = "Me"
            };

            dbContext.RawIngredients.Add(createdRawIngredient);
            await dbContext.SaveChangesAsync();

            var logger = loggerFactory.CreateLogger("Raw Ingredient");
            logger.LogInformation("Raw Ingredient: {IngredientName} added to database", createdRawIngredient.Name);

            return Results.CreatedAtRoute(GetRawIngredient, new { ingredientId = createdRawIngredient.Id }, new NewlyCreatedRawIngredientResponseDto(createdRawIngredient.Id, createdRawIngredient.Name));

        }).WithParameterValidation().DisableAntiforgery().AllowAnonymous();

        app.MapPut("/{ingredientId}", async (FitnessAssistantContext dbContext, Guid ingredientId, 
        [FromForm] UpdatePutRawIngredientReqDto updateRawIngredientReq, FileUploader fileUploader, ClaimsPrincipal userClaim) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) return Results.Unauthorized();

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ??
                                            userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) return Results.Unauthorized();

            var existingRawIngredient = await dbContext.RawIngredients.FindAsync(ingredientId);
            if (existingRawIngredient is null) { return Results.NotFound(); }

            var imageUri = existingRawIngredient.ProfileImageUri;
            if (updateRawIngredientReq.ImageFile is not null)
            {
                var fileUploadResult = await fileUploader.UploadFileAsync(updateRawIngredientReq.ImageFile, StorageNames.RawIngredientImageFolder);
                if (!fileUploadResult.IsSuccess)
                {
                    return Results.BadRequest(new { message = fileUploadResult.ErrorMessage });
                }
                imageUri = fileUploadResult.FileUrl;
            }

            existingRawIngredient.Name = updateRawIngredientReq.Name;
            existingRawIngredient.CaloriesPerUnit = updateRawIngredientReq.Calories;
            existingRawIngredient.ProteinPerUnit = updateRawIngredientReq.Protein;
            existingRawIngredient.BaseLineMeasurement = updateRawIngredientReq.BaseLineMeasurement;
            existingRawIngredient.UnitOfMeasurement = updateRawIngredientReq.UnitOfMeasurement;
            existingRawIngredient.FoodGroupId = updateRawIngredientReq.FoodGroup;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();

        }).WithParameterValidation().DisableAntiforgery();

        app.MapPatch("/{ingredientId}", async (FitnessAssistantContext dbContext, Guid ingredientId, HttpRequest request, FileUploader fileUploader, IMapper mapper) =>
        {

            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            var patchRequestBody = JsonConvert.DeserializeObject<JsonPatchDocument<UpdatePatchRawIngredientReqDto>>(body);

            if (patchRequestBody == null) return Results.BadRequest();
            var existingRawIngredient = await dbContext.RawIngredients.FindAsync(ingredientId);
            if (existingRawIngredient == null) return Results.NotFound();

            var dtoToPatch = mapper.Map<UpdatePatchRawIngredientReqDto>(existingRawIngredient);

            var errors = new List<ValidationResult>();
            patchRequestBody.ApplyTo(dtoToPatch);

            var context = new ValidationContext(dtoToPatch);
            if (!Validator.TryValidateObject(dtoToPatch, context, errors, true))
            {
                return Results.ValidationProblem(errors.ToDictionary(e => e.MemberNames.First(), e => new[] { e.ErrorMessage })!);
            }

            var result = mapper.Map(dtoToPatch, existingRawIngredient);

            await dbContext.SaveChangesAsync();

            return Results.NoContent();

        }).AllowAnonymous().DisableAntiforgery();

        app.MapDelete("/ingredientId", async (FitnessAssistantContext dbContext, Guid ingredientId) =>
        {
            await dbContext.RawIngredients.Where(ingredient => ingredient.Id == ingredientId).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization(Policies.AdminAccess);
    }
}
