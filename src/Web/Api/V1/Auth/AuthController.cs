using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiShared.Authorization;
using ApiShared.Dto.Auth;
using Application.Services.Accounts;
using Application.Services.Owners;
using Application.Services.Time;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

[Route("api/v1/")]
public class AuthApi : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly ITimeService _timeService;
    private readonly IOwnerService ownerService;

    public AuthApi(
        IAccountService accountService,
        IOwnerService ownerService,
        TimeFreezeService timeService)
    {
        this.accountService = accountService;
        this.ownerService = ownerService;
        _timeService = timeService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrEmpty(parameters.Email), $"Invalid {nameof(parameters.Email)}");
        Check.Contract(!string.IsNullOrEmpty(parameters.Password), $"Invalid {nameof(parameters.Password)}");

        var accountOrNull = await accountService.GetAccountByEmailAsync(parameters.Email, cancellationToken);
        Check.Contract(accountOrNull, "Invalid email or Password was entered");
        var account = accountOrNull!;

        Check.Contract(accountService.IsPasswordCorrect(parameters.Password, account),"Invalid email or Password was entered");
        
        // TODO: Check if account.isDeleted is false
        
        var claims = new List<Claim>
        {
            new Claim("Id", account.Id.ToString()),
            new Claim("Role", account.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email),
            new Claim("OwnerId", account.OwnerId.ToString()),
            new Claim("TokenVersion", account.TokenVersion.ToString())
        };

        if (account.Role == Core.Entities.Accounts.AccountRole.Admin)
        {
            claims.Add(new Claim(IdentityData.AdminUserClaimName, "true"));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _timeService.GetUtcTime().Add(AuthSharedConfiguration.AccessTokenLifetime),
            Issuer = $"{Request.Scheme}://{Request.Host}",
            Audience = $"{Request.Scheme}://{Request.Host}",
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

        Response.Cookies.Append(AuthSharedConfiguration.AccessTokenKey, cookieStringToken, cookieOptions);
        
        return Ok(new LoginResult { Token = cookieStringToken });
    }
    
    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var tokenString = HttpContext.Request.Headers["Authorization"].ToString();
        var rawToken = tokenString.Replace("Bearer ", "");
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(rawToken);

        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        Check.Contract(!string.IsNullOrEmpty(email), "Invalid token: missing email claim");

        var account = await accountService.GetAccountByEmailAsync(email!, cancellationToken);
        Check.Contract(account, "Account not found.");

        account!.TokenVersion++;
        await accountService.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }
}