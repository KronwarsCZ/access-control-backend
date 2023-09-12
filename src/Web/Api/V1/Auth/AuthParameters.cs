using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

public static class AuthParameters
{
    public static TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "https://myfunwebsite.com",
        ValidAudience = "https://myfunwebsite.com",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mujsuprcuprtokensecret")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
}
