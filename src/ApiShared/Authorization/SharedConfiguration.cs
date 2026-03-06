namespace ApiShared.Authorization;

public static class AuthSharedConfiguration
{
    private const string DefaultDevSecretKey =
        "mujsuprcuprtokensecret_this_is_a_very_long_secret_key_that_is_at_least_64_bytes_long_for_hmac_sha512_algorithm_2024";

    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromHours(6);
    public static string AccessTokenKey = "AccessControlJwt";
    public static string TokenSecretKey =
        Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? DefaultDevSecretKey;
}
