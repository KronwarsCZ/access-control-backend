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
        
        return Ok();
    }
    
    [HttpDelete]
    [Route("roles/deleteRoles")]
    public async Task<ActionResult> DeleteRolesAsync([FromBody] RoleDeleteParameters parameters, CancellationToken cancellationToken)
    {
        return Ok();
    }
    
    [HttpPut]
    [Route("roles/updateRole")]
    public async Task<ActionResult> UpdateRoleAsync([FromBody] RoleUpdateParameters roleParams, CancellationToken cancellationToken)
    {
        
        return Ok();
    }
    
    [HttpPost]
    [Route("roles/addRoles")]
    public async Task<ActionResult<IEnumerable<Guid>>> AddRoles([FromBody] RoleAddParameters rolesParameters,
        CancellationToken cancellationToken)
    {
        return Ok();
    }
}
