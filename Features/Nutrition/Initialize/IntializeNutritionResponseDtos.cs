public record AdminInitializeResponseDto(
    IEnumerable<FoodGroup> FoodGroups,
    IEnumerable<MealCategory> MealCategories
);