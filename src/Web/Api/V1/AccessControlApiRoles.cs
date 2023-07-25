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

        if (roles is null)
        {
            return NoContent();
        }
        return Ok(roles);
    }
    
    [HttpDelete]
    [Route("roles/deleteRoles")]
    public async Task<ActionResult<(int, IEnumerable<string>)>> DeleteRolesAsync(
        [FromBody] Dto.RolesFilterParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters?.Ids is null)
        {
            return UnprocessableEntity();
        }

        var result = await _roleService.DeleteAsync(parameters.Ids, cancellationToken);

        if (result is { Item1: 0, Item2: null })
        {
            return BadRequest();
        }

        return Ok(result.Item1);
    }
    
    [HttpPut]
    [Route("roles/updateRole")]
    public async Task<ActionResult<Key>> UpdateRoleAsync(Guid id, [FromBody] Dto.RoleAddParameters roleParams, CancellationToken cancellationToken)
    {
        if (roleParams is null)
        {
            return BadRequest();
        }

        var roleFromParams = roleParams.Roles.First();

        var role = _roleService.Create(roleFromParams.Id, roleFromParams.Name, roleFromParams.IsDeleted);
        
        var result = await _roleService.UpdateAsync(id, role, cancellationToken);

        if (result is null)
        {
            return BadRequest();
        }

        return Ok(result);
    }
    
    [HttpPost]
    [Route("roles/addRoles")]
    public async Task<ActionResult<IEnumerable<Key>>> AddRoles( [FromBody] Dto.RoleAddParameters rolesParameters,
        CancellationToken cancellationToken)
    {
        if (rolesParameters is null || !rolesParameters.Roles.Any())
        {
            return BadRequest();
        }

        var roles = rolesParameters.Roles
            .Select(k => _roleService.Create(k.Id, k.Name, k.IsDeleted));
        
        var result = await _roleService.AddAsync(roles, cancellationToken);

        return result is null ? Problem() : Ok(result);
    }
}
