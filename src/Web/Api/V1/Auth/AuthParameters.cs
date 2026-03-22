using System.Text;
using ApiShared.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

public static class AuthParameters
{
    public static TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSharedConfiguration.TokenSecretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerValidator = (issuer, token, parameters) =>
        {
            if (Uri.TryCreate(issuer, UriKind.Absolute, out _))
                return issuer;
            throw new SecurityTokenInvalidIssuerException($"Invalid issuer: {issuer}");
        },
        AudienceValidator = (audiences, token, parameters) =>
        {
            return audiences.Any(a => Uri.TryCreate(a, UriKind.Absolute, out _));
        }
    };
}
