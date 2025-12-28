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
    public List<Guid> MealIngredientsId { get; set; } = [];

    public int TotalCalories{get;set;}
    public double TotalProtein{get;set;}
    public double TotalFoodOutputQuantity{get;set;}
    public UnitOfMeasurement TotalFoodMeasurementUnit{get;set;}
    public double NumberOfServing{get;set;}
    public double ServingSizePerMeasuringUnit{get;set;}

    public int CaloriesPerServing{get;set;}
    public double ProteinPerServing{get;set;}
    public PlateSize LargePlateSize{get;set;}
    public PlateSize SmallPlateSize{get;set;}
    public PlateSize BowlSize{get;set;}



    public NutritionContributor? MealSubmitter { get; set; }
    public required Guid MealSubmitterId { get; set; }
    public int MostRecentMealCreationStage { get; set; } = 1;
}