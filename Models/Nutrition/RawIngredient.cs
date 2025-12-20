public class RawIngredient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required int CaloriesPerUnit { get; set; }
    public required double ProteinPerUnit { get; set; }

    public required UnitOfMeasurement UnitOfMeasurement { get; set; }
    public required int BaseLineMeasurement { get; set; }


    public int RecipeCount { get; set; } = 0;

    public FoodGroup? FoodGroup { get; set; }
    public Guid FoodGroupId { get; set; }



    public NutritionContributor? SubmittedBy { get; set; }
    public Guid SubmittedById { get; set; }

    public required string ProfileImageUri { get; set; }
    public required string LastUpdatedBy { get; set; }
}