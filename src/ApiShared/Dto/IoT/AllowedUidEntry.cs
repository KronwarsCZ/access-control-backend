namespace ApiShared.Dto.IoT;

/// <summary>
/// Represents a single allowed UID entry for access control.
/// </summary>
public class AllowedUidEntry
{
    /// <summary>
    /// Hex-encoded UID (4-16 hex characters).
    /// </summary>
    public string Uid { get; init; } = null!;

    /// <summary>
    /// Start validity as Unix timestamp. 0 = no restriction (immediate).
    /// </summary>
    public long ValidFrom { get; init; }

    /// <summary>
    /// End validity as Unix timestamp. 0 = never expires.
    /// </summary>
    public long ValidTo { get; init; }
}
