namespace ApiShared.Dto.Keys;

public class GetAllKeysResult
{
    public IEnumerable<KeyResult> Keys { get; init; } = null!;
}
