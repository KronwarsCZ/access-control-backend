namespace ApiShared.Dto.Accounts;

public class CreateAccountResult
{
    public Guid AccountId { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
