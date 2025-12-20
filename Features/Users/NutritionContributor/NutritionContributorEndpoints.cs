using FitnessAssistant.Api.Data;

public static class NutritionContributorEndpoints
{
    private const string DefaultProfileImageUri = "https://placeholder.co/100";
    public static void MapNutritionContributorEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapGet("/", async (FitnessAssistantContext dbContext) =>
        {

        }).WithName(NutritionContributorEndpointConstants.GetNutritionContributor);

        app.MapGet("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {

        });

        app.MapPost("/", async (FitnessAssistantContext dbContext) =>
        {
            NutritionContributor createdContributor = new NutritionContributor()
            {
                Name = "Test Contributor",
                ProfileImageUri = DefaultProfileImageUri
            };

            dbContext.NutritionContributors.Add(createdContributor);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(NutritionContributorEndpointConstants.GetNutritionContributor,
            new { id = createdContributor.Id }, new CreatedNutritionContributorResponseDto(createdContributor.Id, createdContributor.Name));
        }).AllowAnonymous();

        app.MapPut("/", async (FitnessAssistantContext dbContext) =>
        {

        });

        app.MapDelete("/", async (FitnessAssistantContext dbContext) =>
        {

        });
    }
}