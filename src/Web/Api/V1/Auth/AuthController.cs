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
            }),
            Expires = _timeService.GetUtcTime().Add(AuthSharedConfiguration.AccessTokenLifetime),
            Issuer = AuthSharedConfiguration.Issuer,
            Audience = AuthSharedConfiguration.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthSharedConfiguration.TokenSecretKey)), SecurityAlgorithms.HmacSha512Signature),
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();

        // generate refresh cookie
        var cookieToken = tokenHandler.CreateToken(tokenDescriptor);
        var cookieStringToken = tokenHandler.WriteToken(cookieToken);

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
    
    
    [HttpGet("refresh")]
    public async Task<ActionResult> GetAccessToken(CancellationToken cancellationToken)
    {
        var refreshTokenCookie = Request.Cookies[AuthSharedConfiguration.AccessTokenKey];
        if (refreshTokenCookie is null)
        {
            return BadRequest();
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = AuthParameters.TokenValidationParameters;

        SecurityToken validatedToken;
        tokenHandler.ValidateToken(refreshTokenCookie, validationParameters, out validatedToken);
        
        var jwtToken = tokenHandler.ReadJwtToken(refreshTokenCookie);
        
        var id = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var role = jwtToken.Claims.FirstOrDefault(x => x.Type == "Role")?.Value;
        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", id),
                new Claim("Role", role),
                new Claim(JwtRegisteredClaimNames.Email, email),
            }),
            Expires = DateTime.UtcNow.Add(AuthSharedConfiguration.AccessTokenLifetime),
            Issuer = AuthSharedConfiguration.Issuer,
            Audience = AuthSharedConfiguration.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthSharedConfiguration.TokenSecretKey)), SecurityAlgorithms.HmacSha512Signature),
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        
        return Ok(stringToken);
    }
    
}