using FitnessAssistant.Api.Data;

public static class InitializeNutritionEndpoints
{
    // private const string GetFoodMeal = nameof(GetFoodMeal);
    public static void MapInitializeNutritionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/user", async (FitnessAssistantContext dbContext) =>
        {
            //items to come from the db for prefetched user login
        });

        app.MapGet("/contributor", async (FitnessAssistantContext dbContext) =>
        {
            //items to come from the db for prefetched nutrition contributor login
        });

        app.MapGet("/admin", async (FitnessAssistantContext dbContext) =>
        {
            //items to come from the db for prefetched nutrition admin login
        });
    }
}
