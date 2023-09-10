using AccessControl.Web.Api.V1.Auth.Dto;
using AccessControl.Web.Api.V1.Dto.Devices;
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
    public async Task<ActionResult> Login([FromBody] LoginParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrEmpty(parameters.Email), $"Invalid {nameof(parameters.Email)}");
        Check.Contract(!string.IsNullOrEmpty(parameters.Password), $"Invalid {nameof(parameters.Password)}");

        var account = await accountService.GetAccountByEmailAsync(parameters.Email, cancellationToken);

        if (account is null)
        {
            BadRequest();
        }
        
        var cookieOptions = new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            HttpOnly = true, // true means it cannot be reached from javascript code
            Secure = true,
        };

        Response.Cookies.Append("AccessToken", "MyGloriousAccessToken", cookieOptions);

        return Ok();
    }
}