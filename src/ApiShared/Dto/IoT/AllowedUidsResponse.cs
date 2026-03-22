namespace ApiShared.Dto.IoT;

/// <summary>
/// JSON response for allowed UIDs list.
/// Alternative to binary format.
/// </summary>
public class AllowedUidsResponse
{
    /// <summary>
    /// Unix timestamp of this response.
    /// </summary>
    public long Timestamp { get; init; }

    /// <summary>
    /// List of allowed UID entries.
    /// </summary>
    public IEnumerable<AllowedUidEntry> Entries { get; init; } = null!;
}
