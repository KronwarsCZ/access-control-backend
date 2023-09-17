using System.Globalization;
using ApiShared.Dto.Devices;
using ApiShared.Dto.Keys;
using ApiShared.Dto.Time;
using Core.Entities.Devices;
using Core.Entities.Keys;
using Framework.Extensions;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("keys/issueNewKey")]
    public async Task<ActionResult<IssueNewKeyResult>> IssueNewKey([FromBody] IssueNewKeyParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.ValidityStartUtc < parameters.ValidityEndUtc, "Validity cannot end before start.");

        Check.Contract(parameters.ValidityEndUtc is null || parameters.ValidityEndUtc > DateTime.UtcNow, "Key validity cannot end in the past.");
        
        var key = Key.Create(auth.Owner, parameters.Name, parameters.ValidityStartUtc, parameters.ValidityEndUtc);
        
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
    [Route("keys/getAllKeys")]
    public async Task<ActionResult<GetAllKeysResult>> GetAllKeys([FromBody] GetAllKeysParameters parameters, CancellationToken cancellationToken)
    {
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
    public async Task<ActionResult<KeyDeleteResult>> DeleteRole([FromBody] KeyDeleteParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}");

        await keyService.DeleteAsync(key, cancellationToken);
        await keyService.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("keys/getAllUnlockingDevices")]
    public async Task<ActionResult<GetAllUnlockingDevicesResult>> GetAllUnlockingDevices([FromBody] GetAllUnlockingDevicesParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        var key = await keyService.GetByIdAsync(parameters.KeyId, cancellationToken);
        Check.Contract(key.OwnerId == auth.Owner.Id, $"Invalid {nameof(parameters.KeyId)}");
        var devicesToOpen = await keyService.GetDevicesToOpen(key, cancellationToken);

        var deviceResults = devicesToOpen.Select(Serialize);
        return Ok(new GetAllUnlockingDevicesResult
        {
            Devices = deviceResults 
            
        });
    }
    
    private DeviceResult Serialize(Device device)
    {
        return new DeviceResult
        {
            Id = device.Id,
            Name = device.Name,
            Type = device.Type.ToString(),
            CreatedUtc = device.CreatedUtc,
            UpdatedUtc = device.UpdatedUtc
        };
    }
}