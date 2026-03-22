namespace ApiShared.Dto.Roles;

public class RoleRenameResult
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = null!;
}
