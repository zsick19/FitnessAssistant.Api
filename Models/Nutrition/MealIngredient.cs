public class MealIngredient
{
    public Guid Id { get; set; }
    public RawIngredient? RawIngredient { get; set; }
    public Guid RawIngredientId { get; set; }

    public required int QuantityUsed { get; set; }
    public required UnitOfMeasurement UnitOfMeasurement { get; set; }
    public required int TotalCaloriesPerQuantityUsed { get; set; }

    public Guid FoodMealId { get; set; }
}