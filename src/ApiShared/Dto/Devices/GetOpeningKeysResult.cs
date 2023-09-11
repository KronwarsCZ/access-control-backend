namespace AccessControl.Web.Api.V1.Dto.Devices;

public class GetOpeningKeysResult
{
    public IEnumerable<OpeningKey> OpeningKeys { get; init; }
}

public class OpeningKey
{
    public Guid KeyId { get; init; }
    public string Token { get; init; }
}
