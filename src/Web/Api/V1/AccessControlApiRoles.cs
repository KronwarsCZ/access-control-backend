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
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");
        var roleName = parameters.Name;
        var newRole = Role.Create(auth.Owner, roleName);
        await roleService.AddAsync(newRole.ToEnumerable(), cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        var roleAddResult = new RoleAddResult
        {
            Name = newRole.Name,
            RoleId = newRole.Id
        };
        return Ok(roleAddResult);
    }
    
    [HttpDelete]
    [Route("roles/deleteRole")]
    public async Task<ActionResult<RoleDeleteResult>> DeleteRole([FromBody] RoleDeleteParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        await roleService.DeleteAsync(role.ToEnumerable(), cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);
        
        return NoContent();
    }

    [HttpPost]
    [Route("roles/getAllRoles")]
    public async Task<ActionResult<GetAllRolesResult>> GetAllRoles([FromBody] GetAllRolesParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var roles = await roleService.GetAllAsync(auth.Owner, cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        var roleAddResult = roles.Select(r => new RoleResult
        {
            Id = r.Id,
            Name = r.Name,
            CreatedUtc = r.CreatedUtc,
            UpdatedUtc = r.UpdatedUtc,
            IsDeleted = r.IsDeleted
        });

        return Ok(roleAddResult);
    }
}
