using FitnessAssistant.Api.Features.Nutrition.Category;
using FitnessAssistant.Api.Features.Nutrition.FoodMeals;
using FitnessAssistant.Api.Features.Nutrition.RawIngredients;

public static class ApplicationRoutes
{
    public static void MapAllRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/user").MapUserEndpoints();



        app.MapGroup("/contributor/nutrition").MapNutritionContributorEndpoints();



        app.MapGroup("/nutrition/foodMeal").MapFoodMealEndpoints();

        app.MapGroup("/nutrition/initialize").MapInitializeNutritionEndpoints();
        app.MapGroup("/nutrition/foodGroup").MapFoodGroupEndpoints();
        app.MapGroup("/nutrition/ingredients").MapRawIngredientEndpoints();
        app.MapGroup("/nutrition/mealCategory").MapMealCategoriesEndpoints();
    }
}

