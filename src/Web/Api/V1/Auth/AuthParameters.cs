using System.Text;
using ApiShared.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

public static class AuthParameters
{
    public static TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = AuthSharedConfiguration.Issuer,
        ValidAudience = AuthSharedConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSharedConfiguration.TokenSecretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
}
