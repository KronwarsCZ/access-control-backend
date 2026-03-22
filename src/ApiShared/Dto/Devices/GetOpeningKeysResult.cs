namespace ApiShared.Dto.Devices;

public class GetOpeningKeysResult
{
    public IEnumerable<OpeningKey> OpeningKeys { get; init; } = null!;
}

public class OpeningKey
{
    public Guid KeyId { get; init; }
    public string KeyUid { get; init; } = null!;
}
