using FitnessAssistant.Api.Data;
using Microsoft.EntityFrameworkCore;


namespace FitnessAssistant.Api.Features.Nutrition.Category;

public static class MealCategoryEndpoints
{
    private const string GetMealCategory = nameof(GetMealCategory);
    public static void MapMealCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (FitnessAssistantContext dbContext) =>
        {
            var foundFoodMealCategories = await dbContext.MealCategories.OrderBy(cat => cat.Name).ToListAsync();
            return new FoodMealCategoriesPageResponseDto(foundFoodMealCategories);
        }).AllowAnonymous();

        app.MapGet("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            MealCategory? foundMealCategory = await dbContext.MealCategories.FindAsync(id);
            return foundMealCategory is null ? Results.NotFound() : Results.Ok(foundMealCategory);
        }).WithName(GetMealCategory).AllowAnonymous();



        app.MapPost("/", async (FitnessAssistantContext dbContext) =>
        {

        }).AllowAnonymous();

        app.MapPut("/", async (FitnessAssistantContext dbContext) =>
        {

        });

        app.MapDelete("/", async (FitnessAssistantContext dbContext) =>
        {

        });


    }
}


