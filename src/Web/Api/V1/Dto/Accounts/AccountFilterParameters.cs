namespace AccessControl.Web.Api.V1.Dto.Accounts;

public class AccountFilterParameters
{
    public IEnumerable<Guid> Ids { get; set; }
    public string Email { get; init; }
}
