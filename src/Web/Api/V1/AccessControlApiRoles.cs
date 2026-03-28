using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using ApiShared.Dto.Roles;
using ApiShared.Dto.Time;
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
    [Route("roles/renameRole")]
    public async Task<ActionResult<RoleRenameResult>> RenameRole([FromBody] RoleRenameParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.NewName), $"{nameof(parameters.NewName)} cannot be empty.");

        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(role.OwnerId == auth.Owner.Id, "Invalid Owner and Role");

        await roleService.RenameAsync(role, parameters.NewName, cancellationToken);
        await roleService.SaveChangesAsync(cancellationToken);

        return Ok(new RoleRenameResult { RoleId = role.Id, Name = role.Name });
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
        //TODO: Umm zajimavy return type
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var role = await roleService.GetByIdAsync(parameters.Id, cancellationToken);
        Check.Contract(role, $"Invalid {parameters.Id}");

        return Ok(new GetAllRolesResult()
        {
            Roles = Serialize(role).ToEnumerable()
        });
    }
    
    [HttpPost]
    [Route("roles/getRolesForKey")]
    public async Task<ActionResult<GetAllRolesResult>> GetRolesForKey([FromBody] GetRolesForKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        var key = await keyService.GetByIdWithRolesAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}.");
        var roles = key.Roles;

        var roleAddResult = new GetAllRolesResult()
        {
            Roles = roles.Select(Serialize)
        };

        return Ok(roleAddResult);
    }

    [HttpPost]
    [Route("roles/getKeysForRole")]
    public async Task<ActionResult<GetAllKeysResult>> GetKeysForRole([FromBody] GetKeysForRoleParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");
        var keys = await roleService.GetKeysForRoleAsync(parameters.RoleId, auth.Owner.Id, cancellationToken);
        Check.Contract(keys is not null, $"Invalid {nameof(parameters.RoleId)}");

        return Ok(new GetAllKeysResult
        {
            Keys = keys!.Select(k => new KeyResult
            {
                Id = k.Id,
                Name = k.Name,
                KeyUid = k.KeyUid,
                CreatedUtc = k.CreatedUtc,
                UpdatedUtc = k.UpdatedUtc,
                Validity = new DateTimeSpanResult
                {
                    StartUtc = k.ValidityStartUtc,
                    EndUtc = k.ValidityEndUtc
                }
            })
        });
    }

    [HttpPost]
    [Route("roles/getDevicesForRole")]
    public async Task<ActionResult<GetAllDevicesResult>> GetDevicesForRole([FromBody] GetDevicesForRoleParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");
        var devices = await roleService.GetDevicesForRoleAsync(parameters.RoleId, auth.Owner.Id, cancellationToken);
        Check.Contract(devices is not null, $"Invalid {nameof(parameters.RoleId)}");

        return Ok(new GetAllDevicesResult
        {
            Devices = devices!.Select(d => new DeviceResult
            {
                Id = d.Id,
                Name = d.Name,
                HardwareId = d.HardwareId,
                Type = d.Type.ToString(),
                CreatedUtc = d.CreatedUtc,
                UpdatedUtc = d.UpdatedUtc
            })
        });
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
