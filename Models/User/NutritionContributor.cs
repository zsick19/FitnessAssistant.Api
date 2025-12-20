public class NutritionContributor
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string ProfileImageUri { get; set; }
    public bool Active { get; set; } = true;
    public List<FoodMeal> ContributedFoodMeals { get; set; } = [];
    public List<RawIngredient> ContributedRawIngredients { get; set; } = [];

}