public record GetAllContributorPageDto(int totalPages, IEnumerable<NutritionContributorSummaryResponseDto> Data);
public record NutritionContributorSummaryResponseDto(Guid id, string Name);
public record NutritionContributorDetailResponseDto(Guid id, string Name, string ProfileImageUri);
public record CreatedNutritionContributorResponseDto(Guid id, string Name);