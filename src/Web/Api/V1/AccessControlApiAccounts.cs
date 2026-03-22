using ApiShared.Dto.Auth;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("account/changePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        await accountService.ChangePasswordAsync(auth.Account, parameters.CurrentPassword, parameters.NewPassword, cancellationToken);

        return Ok();
    }
}
