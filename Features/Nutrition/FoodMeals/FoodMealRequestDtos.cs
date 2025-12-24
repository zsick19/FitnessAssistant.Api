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
