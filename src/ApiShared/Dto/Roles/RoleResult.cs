namespace ApiShared.Dto.Roles;

public class RoleResult
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
    public bool IsDeleted { get; init; }
}
