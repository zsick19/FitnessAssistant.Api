public record FoodMealsPageResponseDto(int totalPages, IEnumerable<FoodMealSummaryResponseDto> Data);

public record FoodMealSummaryResponseDto(
    Guid Id,
    string Name,
    string Description,
    string SubmitterName
);

public record FoodMealCreationResponseDto(
    string Id,
    string Name,
    string Description,
    string MealCategoryId,
    List<string> submittedPhotos
);