using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
    private readonly IIoTHubDeviceService ioTHubDeviceService;

    public AccessControlApi(
        IDeviceService deviceService,
        IOwnerService ownerService,
        IKeyService keyService,
        IAccountService accountService,
        IRoleService roleService,
        IIoTHubDeviceService ioTHubDeviceService)
    {
        this.deviceService = deviceService;
        this.ownerService = ownerService;
        this.keyService = keyService;
        this.roleService = roleService;
        this.accountService = accountService;
        this.ioTHubDeviceService = ioTHubDeviceService;
    }

    private async Task<AuthorizationContext> GetAuthorizationContextFromJwtTokenAsync(HttpContext httpContext, CancellationToken cancellationToken)
    {
        if (httpContext.Items["AuthenticatedAccount"] is Account cachedAccount)
        {
            Check.Contract(cachedAccount.Owner, "Invalid Jwt Token");
            Check.Contract(cachedAccount.Owner.Id != Guid.Empty, "Invalid Jwt Token");
            return new AuthorizationContext(cachedAccount.Owner, cachedAccount);
        }

        var tokenString = HttpContext.Request.Headers["Authorization"].ToString();
        var tokenHandler = new JwtSecurityTokenHandler();

        var rawToken = tokenString.Replace("Bearer ", "");
        var jwtToken = tokenHandler.ReadJwtToken(rawToken);
        
        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        Check.Contract(!string.IsNullOrEmpty(email), "Invalid Jwt Token");

        var account = await accountService.GetAccountByEmailAsync(email!, cancellationToken);
        Check.Contract(account, "Invalid Jwt Token");
        Check.Contract(account!.Owner, "Invalid Jwt Token");
        Check.Contract(account.Owner.Id != Guid.Empty, "Invalid Jwt Token");

        return new AuthorizationContext(account.Owner, account);
    }

    private static async Task<string> GetRawBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanSeek) request.EnableBuffering();

        request.Body.Position = 0;
       
        var reader = new StreamReader(request.Body, Encoding.UTF8, false, 4096, true);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        reader.Close();
        
        request.Body.Position = 0;
        return body;
    }
    
    private static void RequireAdmin(AuthorizationContext auth)
    {
        Check.Contract(auth.Account.Role == AccountRole.Admin || auth.Account.Role == AccountRole.Owner,
            "Insufficient permissions. This action requires administrator privileges.");
    }

    private record AuthorizationContext(Owner Owner, Account Account);
}
