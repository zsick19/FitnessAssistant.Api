using System.ComponentModel.DataAnnotations;

public record GetFoodMealSummaryPageRequestDto(int pageNumber = 1, int pageSize = 5, string? searchMealName = null, bool searchByGuid = false);








public record CreateFoodMealRequestDto(
    [Required] string Name,
    [Required] string Description,
    [Required] Guid MealCategoryId
    )
{
    public IFormFileCollection? SubmissionPhotos { get; set; }
    public List<string>? Instructions { get; set; }
    public List<IngredientAmountRequestDto>? IngredientsUsed { get; set; }
};

public record IngredientAmountRequestDto(
    Guid IngredientId,
    double QuantityUsed,
    UnitOfMeasurement UnitOfMeasurement
);






public record UpdateFoodMealRequestDto([Required] string Name)
{
    public IFormFile? SubmissionPhoto { get; set; }
}


public record UpdatePatchFoodMealRequestDto
{
    public string? name { get; set; }
    public string? description { get; set; }
    public List<MealIngredient>? mealIngredients { get; set; }
    public int? mostRecentMealCreationStage { get; set; }

    public int? TotalCalories{get;set;}
    public double? TotalProtein{get;set;}
    public double? TotalFoodOutputQuantity{get;set;}
    public UnitOfMeasurement? TotalFoodMeasurementUnit{get;set;}
    public double? NumberOfServing{get;set;}
    public double? ServingSizePerMeasuringUnit{get;set;}

    public int? CaloriesPerServing{get;set;}
    public double? ProteinPerServing{get;set;}
    public PlateSize? LargePlateSize{get;set;}
    public PlateSize? SmallPlateSize{get;set;}
    public PlateSize? BowlSize{get;set;}
}