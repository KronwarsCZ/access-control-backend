using AccessControl.Web.Api.V1.Dto.Accounts;
using Core.Entities;
using Framework.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("test")]
    public async Task<ActionResult<IEnumerable<Role>>> test([FromBody] AccountFilterParameters parameters, CancellationToken cancellationToken)
    {
        var role = roleService.Create("moja rola");
        await roleService.AddAsync(role.ToEnumerable(), cancellationToken);
        return Ok(role);
    }
    
}
