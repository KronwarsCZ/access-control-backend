using ApiShared.Dto.Roles;
using Core.Entities.Roles;
using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("roles/addNewRole")]
    public async Task<ActionResult<RoleAddResult>> AddRoles([FromBody] RoleAddParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        Check.Contract(owner, $"Invalid {nameof(parameters.OwnerId)}");

        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");
        var roleName = parameters.Name;
        var newRole = Role.Create(owner, roleName);
        await roleService.AddAsync(newRole.ToEnumerable(), cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        var roleAddResult = new RoleAddResult
        {
            Name = newRole.Name,
            RoleId = newRole.Id
        };
        return Ok(roleAddResult);
    }
}
