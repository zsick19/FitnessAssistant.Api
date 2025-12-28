using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Shared.Authorization;
using FitnessAssistant.Api.Shared.FileUpload;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FitnessAssistant.Api.Features.Nutrition.FoodMeals;

public static class FoodMealEndpoints
{
    private const string GetFoodMeal = nameof(GetFoodMeal);
    private const string DefaultProfileImageUri = "https://placehold.co/100";

    public static void MapFoodMealEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (FitnessAssistantContext dbContext, [AsParameters] GetFoodMealSummaryPageRequestDto queryRequest, ClaimsPrincipal userClaim) =>
        {
            var skipCount = (queryRequest.pageNumber - 1) * queryRequest.pageSize;

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var filteredMeals = dbContext.FoodMeals.AsQueryable();
            if (queryRequest.searchByGuid && currentUserId != null) { filteredMeals = filteredMeals.Where(meal => meal.MealSubmitterId == new Guid(currentUserId)); }

            filteredMeals = filteredMeals.Where(meal => string.IsNullOrWhiteSpace(queryRequest.searchMealName) || meal.Name.ToLower().Contains(queryRequest.searchMealName.ToLower()) || EF.Functions.Like(meal.Name,
              $"%{queryRequest.searchMealName}"));


            var mealsOnPage = await filteredMeals.OrderBy(meal => meal.Name)
                .Skip(skipCount)
                .Take(queryRequest.pageSize)
                    .Include(meal => meal.MealSubmitter)
                    .Select(meal => new FoodMealSummaryResponseDto(meal.Id, meal.Name, meal.Description, meal.MealSubmitter!.Name))
                    .AsNoTracking()
                    .ToListAsync();

            var totalMeals = await filteredMeals.CountAsync();
            var totalPages = (int)Math.Ceiling(totalMeals / (double)queryRequest.pageSize);

            return new FoodMealsPageResponseDto(totalPages, mealsOnPage);
        }).AllowAnonymous();

        app.MapGet("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            FoodMeal? foundFoodMeal = await dbContext.FoodMeals.Include(p => p.MealIngredients.OrderBy(c => c.RawIngredient!.Name)).ThenInclude(c => c.RawIngredient).FirstOrDefaultAsync(p => p.Id == id);

            return foundFoodMeal is null ? Results.NotFound() : Results.Ok(foundFoodMeal);

        }).WithName(GetFoodMeal);

        app.MapPost("/", async (FitnessAssistantContext dbContext, [FromForm] CreateFoodMealRequestDto createFoodMealRequestDto, ClaimsPrincipal userClaim, FileUploader fileUploader) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }

            FoodMeal createdFoodMeal = new FoodMeal()
            {
                Name = createFoodMealRequestDto.Name,
                Description = createFoodMealRequestDto.Description,
                MealCategoryId = createFoodMealRequestDto.MealCategoryId,
                MealSubmitterId = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348"),
            };

            if (createFoodMealRequestDto.SubmissionPhotos is null || createFoodMealRequestDto.SubmissionPhotos?.Count == 0)
            {
                createdFoodMeal.FinishedMealImageUri.Add(DefaultProfileImageUri);
            }
            else
            {
                foreach (IFormFile file in createFoodMealRequestDto.SubmissionPhotos!)
                {
                    if (file is not null)
                    {
                        var fileUploadResult = await fileUploader.UploadFileAsync(file, "FoodImageFolder");
                        if (!fileUploadResult.IsSuccess)
                        {
                            return Results.BadRequest(new { message = fileUploadResult.ErrorMessage });
                        }
                        createdFoodMeal.FinishedMealImageUri.Add(fileUploadResult.FileUrl!);
                    }
                }
            }

            dbContext.FoodMeals.Add(createdFoodMeal);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetFoodMeal, new { id = createdFoodMeal.Id }, createdFoodMeal.FoodMealToDetailedResDto());

        }).AllowAnonymous().DisableAntiforgery();

        app.MapPatch("/{FoodMealId}", async (Guid FoodMealId, FitnessAssistantContext dbContext, HttpRequest request, IMapper mapper,
        FileUploader fileUploader, ClaimsPrincipal userClaim) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) return Results.Unauthorized();
            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) return Results.Unauthorized();

            var existingFoodMeal = await dbContext.FoodMeals.Include(p => p.MealIngredients).FirstOrDefaultAsync(p => p.Id == FoodMealId);
            if (existingFoodMeal == null) return Results.NotFound();

            var form = await request.ReadFormAsync();
            var body = form["patchDoc"];

            var patchRequestBody = JsonConvert.DeserializeObject<JsonPatchDocument<UpdatePatchFoodMealRequestDto>>(body);
            if (patchRequestBody == null) return Results.BadRequest();

            var dtoToPatch = mapper.Map<UpdatePatchFoodMealRequestDto>(existingFoodMeal);
            var errors = new List<ValidationResult>();
            patchRequestBody.ApplyTo(dtoToPatch);
            var context = new ValidationContext(dtoToPatch);

            if (!Validator.TryValidateObject(dtoToPatch, context, errors, true))
            {
                return Results.ValidationProblem(errors.ToDictionary(e => e.MemberNames.First(), e => new[] { e.ErrorMessage })!);
            }

            var result = mapper.Map(dtoToPatch, existingFoodMeal);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).DisableAntiforgery();

        app.MapDelete("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            await dbContext.FoodMeals.Where(meal => meal.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization(Policies.AdminAccess);
    }
}
