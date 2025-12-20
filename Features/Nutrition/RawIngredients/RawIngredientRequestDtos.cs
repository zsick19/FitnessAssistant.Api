using System.ComponentModel.DataAnnotations;

public record GetRawIngredientsRequestDto(
    int pageNumber = 1,
    int pageSize = 5,
    string? searchName = null
);

public record CreateRawIngredientReqDto(
    [Required] string Name,
    [Required] int Calories,
    [Required] double Protein,
    [Required] UnitOfMeasurement UnitOfMeasurement,
    [Required] int BaseLineMeasurement,
    [Required] Guid FoodGroup
    )
{
    public IFormFile? ImageFile { get; set; }
};


public record UpdatePutRawIngredientReqDto(
    [Required] string Name,
    [Required] int Calories,
    [Required] double Protein,
    [Required] UnitOfMeasurement UnitOfMeasurement,
    [Required] int BaseLineMeasurement,
    [Required] Guid FoodGroup
    )
{
    public IFormFile? ImageFile { get; set; }
};



public record UpdatePatchRawIngredientReqDto()
{
    public string? Name { get; set; }
    public int Calories { get; set; }
    public double Protein { get; set; }
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    public int BaseLineMeasurement { get; set; }
    public Guid FoodGroup { get; set; }
}