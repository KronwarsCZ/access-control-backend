using System.Diagnostics.Contracts;
using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using ApiShared.Dto.Time;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("keys/issueNewKey")]
    public async Task<ActionResult<IssueNewKeyResult>> IssueNewKey([FromBody] IssueNewKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        if (parameters.ValidityEndUtc is not null && parameters.ValidityStartUtc is not null)
        {
            Check.Contract(parameters.ValidityStartUtc < parameters.ValidityEndUtc, "Validity cannot end before start.");
        }

        Check.Contract(parameters.ValidityEndUtc is null || parameters.ValidityEndUtc > DateTime.UtcNow, "Key validity cannot end in the past.");
        
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.KeyUid), $"Invalid {nameof(parameters.KeyUid)}");
        var key = Key.Create(auth.Owner, parameters.Name, parameters.KeyUid, parameters.ValidityStartUtc, parameters.ValidityEndUtc);
        
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"Invalid {nameof(parameters.Name)}");
        
        Check.Contract(parameters.RoleIds, $"Invalid {nameof(parameters.RoleIds)}");
        Check.Contract(parameters.RoleIds.Any(), $"At least one of {nameof(parameters.RoleIds)} has to be specified");
        var roles = await roleService.FindByConditionAsync(r => parameters.RoleIds.Contains(r.Id), cancellationToken);
        Check.Contract(roles.Any(), $"Invalid {nameof(parameters.RoleIds)}");
        Check.Contract(roles.All(r => r.OwnerId == auth.Owner.Id), $"Invalid {nameof(parameters.RoleIds)}");

        
        await keyService.IssueNewKey(key, roles, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        var issueNewKeyResponse = new IssueNewKeyResult
        {
            KeyId = key.Id
        };

        return Ok(issueNewKeyResponse);
    }

    [HttpPost]
    [Route("keys/renameKey")]
    public async Task<ActionResult<KeyRenameResult>> RenameKey([FromBody] KeyRenameParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.KeyId != Guid.Empty, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.NewName), $"{nameof(parameters.NewName)} cannot be empty.");

        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(key.OwnerId == auth.Owner.Id, "Invalid Owner and Key");

        await keyService.RenameAsync(key, parameters.NewName, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        return Ok(new KeyRenameResult { KeyId = key.Id, Name = key.Name });
    }

    [HttpPost]
    [Route("keys/getAllKeys")]
    public async Task<ActionResult<GetAllKeysResult>> GetAllKeys([FromBody] GetAllKeysParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        var keysFromDb = await keyService.GetAllAsync(auth.Owner, cancellationToken);
        
        if (parameters.GetCurrentlyValidOnly)
        {
            keysFromDb = keysFromDb.Where(k => k.ValidityStartUtc <= DateTime.UtcNow && (k.ValidityEndUtc is null || k.ValidityEndUtc > DateTime.UtcNow));
        }

        var keys = new GetAllKeysResult
        {
            Keys = keysFromDb.Select(key => new KeyResult
            {
                Id = key.Id,
                Name = key.Name,
                KeyUid = key.KeyUid,
                CreatedUtc = key.CreatedUtc,
                UpdatedUtc = key.UpdatedUtc,
                Validity = new DateTimeSpanResult
                {
                    StartUtc = key.ValidityStartUtc,
                    EndUtc = key.ValidityEndUtc
                }
            })
        };

        return Ok(keys);
    }
    
    [HttpPost]
    [Route("keys/deleteKey")]
    public async Task<ActionResult<KeyDeleteResult>> DeleteKey([FromBody] KeyDeleteParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        if (key is null) return NoContent();
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}");

        await keyService.DeleteAsync(key, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("keys/getAllUnlockingDevices")]
    public async Task<ActionResult<GetAllUnlockingDevicesResult>> GetAllUnlockingDevices([FromBody] GetAllUnlockingDevicesParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        
        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}");
        
        var devicesToOpen = await keyService.GetDevicesToOpen(key, cancellationToken);

        var deviceResults = devicesToOpen.Select(Serialize);
        return Ok(new GetAllUnlockingDevicesResult
        {
            Devices = deviceResults 
            
        });
    }

    [HttpPost]
    [Route("keys/attachRoleToKey")]
    public async Task<ActionResult> AttachRoleToKey([FromBody] AttachRoleToKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        
        Check.Contract(parameters.KeyId != Guid.Empty, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");
        
        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(key.OwnerId == auth.Owner.Id,
            $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.KeyId)}");
        
        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(role.OwnerId == auth.Owner.Id,
            $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.RoleId)}");

        await keyService.AttachRoleToKey(key, role, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("keys/detachRoleFromKey")]
    public async Task<ActionResult> DetachRoleFromKey([FromBody] DetachRoleFromKeyParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.KeyId != Guid.Empty, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");

        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key, $"Invalid {nameof(parameters.KeyId)}");
        Check.Contract(key.OwnerId == auth.Owner.Id,
            $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.KeyId)}");

        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(role.OwnerId == auth.Owner.Id,
            $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.RoleId)}");

        await keyService.DetachRoleFromKey(key, role, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("keys/getById")]
    public async Task<ActionResult<GetKeyByIdResult>> GetKeyById([FromBody] GetKeyByIdParameters parameters,
        CancellationToken cancellationToken)
    {
        Check.Contract(parameters, "Payload cannot be null");
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(auth.Owner, $"Invalid {nameof(auth.Owner.Id)}");
        Check.Contract(auth.Owner.Id != Guid.Empty, $"Invalid {nameof(auth.Owner.Id)}");

        var key = await keyService.GetByIdAsync(parameters.Id, cancellationToken);
        Check.Contract(key, $"Invalid {parameters.Id}");
        
        return Ok(new GetKeyByIdResult()
        {
            Key = Serialize(key)
        });
    }
    
    private KeyResult Serialize(Key key)
    {
        return new KeyResult()
        {
            Id = key.Id,
            Name = key.Name,
            KeyUid = key.KeyUid,
            CreatedUtc = key.CreatedUtc,
            UpdatedUtc = key.UpdatedUtc,
            Validity = new DateTimeSpanResult()
            {
                StartUtc = key.ValidityStartUtc,
                EndUtc = key.ValidityEndUtc
            }
        };
    }
}