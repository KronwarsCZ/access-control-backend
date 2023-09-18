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
        Check.Contract(parameters, "Payload cannot be null");
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
    
    [HttpPost]
    [Route("roles/deleteRole")]
    public async Task<ActionResult<RoleDeleteResult>> DeleteRole([FromBody] RoleDeleteParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(role.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.RoleId)}");
        await roleService.DeleteAsync(role.ToEnumerable(), cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        return new RoleDeleteResult();
    }
    
    [HttpPost]
    [Route("roles/getAllRoles")]
    public async Task<ActionResult<GetAllRolesResult>> GetAllRoles([FromBody] GetAllRolesParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var roles = await roleService.GetAllAsync(auth.Owner, cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        var roleAddResult = new GetAllRolesResult()
        {
            Roles = roles.Select(r => new RoleResult()
            {
                Id = r.Id,
                Name = r.Name,
                CreatedUtc = r.CreatedUtc,
                UpdatedUtc = r.UpdatedUtc,
                IsDeleted = r.IsDeleted
            })
        };

        return Ok(roleAddResult);
    }
    
    [HttpPost]
    [Route("roles/getById")]
    public async Task<ActionResult<GetAllRolesResult>> GetRoleById([FromBody] GetRoleById parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var role = await roleService.GetByIdAsync(parameters.Id, cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);
        
        return Ok(new GetRoleByIdResult()
        {
            Role = Serialize(role)
        });
    }
    
    [HttpPost]
    [Route("roles/getRolesForKey")]
    public async Task<ActionResult<GetAllRolesResult>> GetRolesForKey([FromBody] GetRolesForKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}.");
        var roles = key.Roles;

        var roleAddResult = new GetAllRolesResult()
        {
            Roles = roles.Select(Serialize)
        };

        return Ok(roleAddResult);
    }

    private RoleResult Serialize(Role role)
    {
        return new RoleResult
        {
            Id = role.Id,
            Name = role.Name,
            CreatedUtc = role.CreatedUtc,
            UpdatedUtc = role.UpdatedUtc,
            IsDeleted = role.IsDeleted
        };
    }
}
