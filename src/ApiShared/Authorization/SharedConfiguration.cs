namespace ApiShared.Authorization;

public static class AuthSharedConfiguration
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromDays(365);
    public static string AccessTokenKey = "AccessControlJwt";
    public static string TokenSecretKey = "mujsuprcuprtokensecret_this_is_a_very_long_secret_key_that_is_at_least_64_bytes_long_for_hmac_sha512_algorithm_2024";
}
