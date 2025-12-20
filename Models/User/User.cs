public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required string ProfileImageUri { get; set; }

    public required string LastUpdatedBy { get; set; }
}