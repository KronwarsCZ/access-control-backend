using Core.Entities;
using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("roles/addNewRole")]
    public async Task<ActionResult<Dto.RoleAddResult>> AddRoles([FromBody] Dto.RoleAddParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");

        var newRole = Role.Create(parameters.Name);
        await roleService.AddAsync(newRole.ToEnumerable(), cancellationToken);

        return Ok(new Dto.RoleAddResult { Name = newRole.Name, RoleId = newRole.Id});
    }
}
