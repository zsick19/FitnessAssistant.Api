public class FoodMeal
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }


    public MealSubmissionStatus MealSubmissionStatus { get; set; } = MealSubmissionStatus.Submitted;
    public MealCategory? MealCategory { get; set; }
    public required Guid MealCategoryId { get; set; }


    public List<string> FinishedMealImageUri { get; set; } = [];
    public List<MealIngredient> MealIngredients { get; set; } = [];

    public NutritionContributor? MealSubmitter { get; set; }
    public required Guid MealSubmitterId { get; set; }

    public int MostRecentMealCreationStage { get; set; } = 1;
}