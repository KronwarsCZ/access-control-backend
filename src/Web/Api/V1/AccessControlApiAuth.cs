using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiShared.Authorization;
using ApiShared.Dto.Auth;
using ApiShared.Dto.Roles;
using Core.Entities.Roles;
using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrEmpty(parameters.Email), $"Invalid {nameof(parameters.Email)}");
        Check.Contract(!string.IsNullOrEmpty(parameters.Password), $"Invalid {nameof(parameters.Password)}");

        var account = await accountService.GetAccountByEmailAsync(parameters.Email, cancellationToken);
        Check.Contract(account, "Invalid email or Password was entered");


        Check.Contract(accountService.IsPasswordCorrect(parameters.Password, account),"Invalid email or Password was entered");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", account.Id.ToString()),
                new Claim("Role", account.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("OwnerId", account.OwnerId.ToString())
            }),
            Expires = _timeService.GetUtcTime().Add(AuthSharedConfiguration.AccessTokenLifetime),
            Issuer = AuthSharedConfiguration.Issuer,
            Audience = AuthSharedConfiguration.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthSharedConfiguration.TokenSecretKey)), SecurityAlgorithms.HmacSha512Signature),
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();

        // generate refresh cookie
        var cookieToken = tokenHandler.CreateToken(tokenDescriptor);
        var cookieStringToken = $"Bearer {tokenHandler.WriteToken(cookieToken)}";

        var cookieOptions = new CookieOptions()
        {
            Expires = _timeService.GetUtcTime().Add(AuthSharedConfiguration.AccessTokenLifetime),
            IsEssential = true,
            HttpOnly = false, // true means it cannot be reached from javascript code
            Secure = true,
        };

        account.SessionToken = cookieStringToken;
        await accountService.SaveChangesAsync(cancellationToken);
        
        Response.Cookies.Append(AuthSharedConfiguration.AccessTokenKey, cookieStringToken, cookieOptions);
        
        return Ok(new LoginResult { Token = cookieStringToken });
    }
    
    [HttpGet]
    [Route("logout")]
    public async Task<ActionResult> Logout([FromBody] LogoutParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Account, "No JWT token was entered.");

        auth.Account.SessionToken = null;
        await accountService.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }
}
