using System.Collections.Immutable;
using AccessControl.Web.Api.V1.Dto.Roles;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("roles/getAll")]
    public async Task<ActionResult<IEnumerable<Role>>> RolesGetAllAsync(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        
        return Ok(roles);
    }
    
    [HttpDelete]
    [Route("roles/deleteRoles")]
    public async Task<ActionResult> DeleteRolesAsync([FromBody] RoleDeleteParameters parameters, CancellationToken cancellationToken)
    {
        var allIds = parameters.Ids;
        
        //TODO: Change to getallbyid
        var roles = await _roleService.GetAllAsync(cancellationToken);
        var rolesToDelete = roles.Where(r => allIds.Contains(r.Id)).ToImmutableArray();

        await _roleService.Delete(rolesToDelete, cancellationToken);

        return Ok();
    }
    
    [HttpPut]
    [Route("roles/updateRole")]
    public async Task<ActionResult> UpdateRoleAsync([FromBody] RoleUpdateParameters roleParams, CancellationToken cancellationToken)
    {
        //TODO
        await _roleService.Update(null, cancellationToken);

        return Ok();
    }
    
    [HttpPost]
    [Route("roles/addRoles")]
    public async Task<ActionResult<IEnumerable<Guid>>> AddRoles([FromBody] RoleAddParameters rolesParameters,
        CancellationToken cancellationToken)
    {
        var roles = rolesParameters.Roles
            .Select(r => _roleService.Create(r.Name)).ToImmutableArray();
        
        var result = await _roleService.AddAsync(roles, cancellationToken);

        return Ok(result);
    }
}
