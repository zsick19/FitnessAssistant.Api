public static class DtoMappingExtensions
{
    public static FoodMealCreationResponseDto FoodMealToDetailedResDto(this FoodMeal meal)
    {
        return new FoodMealCreationResponseDto(
            meal.Id.ToString(),
            meal.Name,
            meal.Description,
            meal.MealCategoryId.ToString(),
            submittedPhotos: meal.FinishedMealImageUri.ToList()
        );
    }
}