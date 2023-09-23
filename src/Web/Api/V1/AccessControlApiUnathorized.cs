using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("unauthorized/msg")]
    public Task<ActionResult<string>> GetRoleById(string parameters, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult<string>>(Ok(($"Got the information {0}!", parameters ?? "null")));
    }
}
