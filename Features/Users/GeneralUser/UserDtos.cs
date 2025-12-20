using System.ComponentModel.DataAnnotations;

public record CreateUserDto(
    [Required] string Name
)
{
    public IFormFile? ImageFile { get; set; }
}

public record UpdateUserDto(
    [Required] string Name
    )
{
    public IFormFile? ImageFile { get; set; }
}

public record GetUsersDto(int pageNumber = 1, int pageSize = 1, string? searchName = null);




// Response DTOs 
public record UserPageDto(int totalPages, IEnumerable<UsersDetailResponseDto> Data);
public record UsersDetailResponseDto(Guid id, string Name, string ImageUri, string LastUpdatedBy);
public record UserSummaryResponseDto(Guid id, string Name, string ImageUri, string LastUpdatedBy);