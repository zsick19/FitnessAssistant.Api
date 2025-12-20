public record RawIngredientsSummaryPageResponseDto(int totalPages, IEnumerable<GetRawIngredientSummaryResponseDto> rawIngredients);
public record GetRawIngredientSummaryResponseDto(
    Guid Id,
    string Name,
    int Calories,
    UnitOfMeasurement UnitOfMeasurement,
    int BaseLineMeasurement,
    string FoodGroup,
    double Protein
    );



public record NewlyCreatedRawIngredientResponseDto(Guid id, string Name);

public record RawIngredientDetailedResponseDto(
    Guid id,
    string Name,
    int Calories,
    double Protein,
    UnitOfMeasurement UnitOfMeasurement,
    int BaseLineMeasurement,
    int RecipeCount,
    string FoodGroup,
    string IngredientImageUri
    );