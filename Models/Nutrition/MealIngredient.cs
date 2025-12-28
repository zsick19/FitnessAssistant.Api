public class MealIngredient
{
    public Guid Id { get; set; }
    public RawIngredient? RawIngredient { get; set; }
    public Guid RawIngredientId { get; set; }

    public required int QuantityUsed { get; set; }

    public required int TotalCalories{get;set;}
    public required double TotalProteinGrams{get;set;}

public required string MeasuringMethod{get;set;}
    public required UnitOfMeasurement measuringUnit { get; set; }

    public Guid FoodMealId { get; set; }
}