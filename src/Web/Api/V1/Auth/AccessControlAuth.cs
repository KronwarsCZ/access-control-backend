using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

public class AccessControlApiAuth : ControllerBase
{
    public AccessControlApiAuth(IAccountService accountService)
    {
        _accountService = accountService;
    }
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan RefreshCookieLifetime = TimeSpan.FromDays(7);
    private readonly IAccountService _accountService;
    private const string RefreshCookieKey = "ACRefreshToken";
    private const string Issuer = "https://myfunwebsite.com";
    private const string Audience = "https://myfunwebsite.com";
    private const string TokenSecretKey = "mujsuprcuprtokensecret";

    [Authorize]
    [HttpGet("authorized")]
    public ActionResult<string> GenerateToken()
    {
        return Ok("yeeeey");
    }

    [HttpGet("refresh")]
    public async Task<ActionResult> GetAccessToken(CancellationToken cancellationToken)
    {
        var refreshTokenCookie = Request.Cookies[RefreshCookieKey];
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
        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", id),
                new Claim(JwtRegisteredClaimNames.Email, email),
            }),
            Expires = DateTime.UtcNow.Add(AccessTokenLifetime),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenSecretKey)), SecurityAlgorithms.HmacSha512Signature),
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        
        return Ok(stringToken);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] Dto.LoginForm form, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetAccountByEmailAsync(form.Email, cancellationToken);
        if (!_accountService.IsPasswordCorrect(form.Password, account))
        {
            return Unauthorized();
        }


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
            }),
            Expires = DateTime.UtcNow.Add(AccessTokenLifetime),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenSecretKey)), SecurityAlgorithms.HmacSha512Signature),
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        // generate refresh cookie
        tokenDescriptor.Expires = DateTime.UtcNow.Add(RefreshCookieLifetime);
        var cookieToken = tokenHandler.CreateToken(tokenDescriptor);
        var cookieStringToken = tokenHandler.WriteToken(cookieToken);

        var cookieOptions = new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            IsEssential = true,
            HttpOnly = true, // true means it cannot be reached from javascript code
            Secure = true,
        };

        Response.Cookies.Append(RefreshCookieKey, cookieStringToken, cookieOptions);
        
        return Ok(stringToken);
    }
}
