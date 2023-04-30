using Core;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("keys/getAll")]
    public ActionResult<string> NecoEndpoint()
    {
        var res = "deploy se povedl, toto je nova verze";
        return Ok(res);
    }
}
