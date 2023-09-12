using ApiShared.Dto.Auth;
using Application.Services.Accounts;
using Application.Services.Owners;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1.Auth;

[Route("api/v1/")]
public partial class AuthApi : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly IOwnerService ownerService;

    public AuthApi(
        IAccountService accountService,
        IOwnerService ownerService)
    {
        this.accountService = accountService;
        this.ownerService = ownerService;
    }
    
    
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrEmpty(parameters.Email), $"Invalid {nameof(parameters.Email)}");
        Check.Contract(!string.IsNullOrEmpty(parameters.Password), $"Invalid {nameof(parameters.Password)}");

        var account = await accountService.GetAccountByEmailAsync(parameters.Email, cancellationToken);

        if (account is null)
        {
            return BadRequest();
        }
        
        var cookieOptions = new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            HttpOnly = true, // true means it cannot be reached from javascript code
            Secure = true,
        };
        var sessionToken = $"AccessToken_{Guid.NewGuid()}-{Guid.NewGuid()}";

        account.SessionToken = sessionToken;
        await accountService.SaveChangesAsync(cancellationToken);
        
        Response.Cookies.Append("AccessToken", sessionToken, cookieOptions);

        return Ok(new LoginResult { Token = sessionToken });
    }
}