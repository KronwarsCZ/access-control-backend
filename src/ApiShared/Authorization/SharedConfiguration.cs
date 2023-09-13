namespace ApiShared.Authorization;

public static class AuthSharedConfiguration
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromDays(365);
    public static string AccessTokenKey = "AccessControlJwt";
    public static string Issuer = "https://myfunwebsite.com";
    public static string Audience = "https://myfunwebsite.com";
    public static string TokenSecretKey = "mujsuprcuprtokensecret";
}
