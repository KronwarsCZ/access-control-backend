using System.IdentityModel.Tokens.Jwt;
using AccessControl.Web.Api.V1.Auth;
using Application.Services.Accounts;
using Application.Services.Devices;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.Services.Roles;
using Core.Entities.Accounts;
using Core.Entities.Owners;
using Framework.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1;

[Authorize]
[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly IDeviceService deviceService;
    private readonly IOwnerService ownerService;
    private readonly IKeyService keyService;
    private readonly IRoleService roleService;
    private readonly IAccountService accountService;

    public AccessControlApi(
        IDeviceService deviceService,
        IOwnerService ownerService,
        IKeyService keyService,
        IAccountService accountService,
        IRoleService roleService)
    {
        this.deviceService = deviceService;
        this.ownerService = ownerService;
        this.keyService = keyService;
        this.roleService = roleService;
        this.accountService = accountService;
    }

    private async Task<AuthorizationContext> GetAuthorizationContextFromJwtTokenAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var tokenString = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.ReadJwtToken(tokenString);
        
        var id = jwtToken.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var role = jwtToken.Claims.FirstOrDefault(x => x.Type == "Role")?.Value;
        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;

        var account = await accountService.GetAccountByEmailAsync(email, cancellationToken);
        Check.Contract(account.SessionToken == tokenString, "InvalidToken");

        return new AuthorizationContext(account.Owner, account);
    }

    private record AuthorizationContext(Owner Owner, Account Account);
}
