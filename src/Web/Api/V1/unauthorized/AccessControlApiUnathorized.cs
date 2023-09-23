using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace AccessControl.Web.Api.V1;

public class UnauthorizedApi : ControllerBase
{
    [HttpPost]
    [Route("unauthorized/msg")]
    public Task<ActionResult<string>> GetRoleById(string parameters)
    {
        string a = $"Got the information: {parameters}!";
        return Task.FromResult<ActionResult<string>>(Ok((a)));
    }
}
