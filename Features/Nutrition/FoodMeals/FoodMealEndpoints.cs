using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Shared.Authorization;
using FitnessAssistant.Api.Shared.FileUpload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessAssistant.Api.Features.Nutrition.FoodMeals;

public static class FoodMealEndpoints
{
    private const string GetFoodMeal = nameof(GetFoodMeal);
    private const string DefaultProfileImageUri = "https://placehold.co/100";

    public static void MapFoodMealEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (FitnessAssistantContext dbContext, [AsParameters] GetFoodMealSummaryPageRequestDto queryRequest) =>
        {
            var skipCount = (queryRequest.pageNumber - 1) * queryRequest.pageSize;

            var filteredMeals = dbContext.FoodMeals.Where(meal => string.IsNullOrWhiteSpace(queryRequest.searchMealName) || EF.Functions.Like(meal.Name, $"%{queryRequest.searchMealName}"));

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
            FoodMeal? foundFoodMeal = await dbContext.FoodMeals.FindAsync(id);

            return foundFoodMeal is null ? Results.NotFound() : Results.Ok(foundFoodMeal);

        }).WithName(GetFoodMeal);

        app.MapPost("/", async (FitnessAssistantContext dbContext, [FromForm] CreateFoodMealRequestDto createFoodMealRequestDto, ClaimsPrincipal userClaim, FileUploader fileUploader) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ??
                                userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }

            FoodMeal createdFoodMeal = new FoodMeal()
            {
                Name = createFoodMealRequestDto.Name,
                Description = createFoodMealRequestDto.Description,
                MealCategoryId = createFoodMealRequestDto.MealCategoryId,
                MealSubmitterId = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348")
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

        app.MapPut("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {

        });

        app.MapDelete("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            await dbContext.FoodMeals.Where(meal => meal.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization(Policies.AdminAccess);
    }
}
