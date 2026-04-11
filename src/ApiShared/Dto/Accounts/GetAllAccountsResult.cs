namespace ApiShared.Dto.Accounts;

public class GetAllAccountsResult
{
    public IEnumerable<AccountResult> Accounts { get; set; } = null!;
}

public class AccountResult
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime CreatedUtc { get; set; }
    public bool CanDelete { get; set; }
}
