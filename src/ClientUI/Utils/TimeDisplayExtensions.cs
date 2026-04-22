namespace DemoRegistrationEncryptionUsingRandomNumberAndSalt.Utils;

public static class TimeDisplayExtensions
{
    public static DateTime AsLocal(this DateTime utc)
    {
        var asUtc = utc.Kind == DateTimeKind.Local
            ? utc.ToUniversalTime()
            : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return asUtc.ToLocalTime();
    }

    public static DateTime? AsLocal(this DateTime? utc) => utc?.AsLocal();

    public static string ToLocalString(this DateTime utc, string format = "g")
        => utc.AsLocal().ToString(format);

    public static string ToLocalString(this DateTime? utc, string format = "g", string fallback = "")
        => utc.HasValue ? utc.Value.ToLocalString(format) : fallback;
}