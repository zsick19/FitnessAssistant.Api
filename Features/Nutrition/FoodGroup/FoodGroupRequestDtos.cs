using System.ComponentModel.DataAnnotations;

public record CreateFoodGroupDto([Required] string Name);
public record UpdateFoodGroupDto([Required] string Name);