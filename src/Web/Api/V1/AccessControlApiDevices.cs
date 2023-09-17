using ApiShared.Dto.Devices;
using ApiShared.Dto.Roles;
using Core.Entities.Devices;
using Framework.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/registerNewDevice")]
    public async Task<ActionResult<DeviceAddResult>> AddDevice([FromBody] DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");

        var newDevice = Device.Create(auth.Owner, parameters.Name, DeviceType.Lock);
        await deviceService.RegisterNewDeviceAsync(newDevice, cancellationToken);
        await deviceService.SaveChangesAsync(cancellationToken);

        return Ok(new DeviceAddResult { DeviceId = newDevice.Id, Type = newDevice.Type.ToString()});
    }

    [HttpPost]
    [Route("devices/attachRoleToDevice")]
    public async Task<ActionResult> AttachRoleToDevice([FromBody] AttachRoleToDeviceParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");    
        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");

        var device = await deviceService.GetByIdAsync(parameters.DeviceId, cancellationToken);
        Check.Contract(device, $"Invalid {nameof(parameters.DeviceId)}");
        Check.Contract(device.OwnerId == auth.Owner.Id, "Invalid Owner and Device");
        
        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role, $"Invalid {nameof(parameters.RoleId)}");
        Check.Contract(role.OwnerId == auth.Owner.Id, "Invalid Owner and Role");

        await deviceService.AttachRoleToDevice(device, role, cancellationToken);
        await deviceService.SaveChangesAsync(cancellationToken);

        return Ok();
    } 
    
    [HttpPost]
    [Route("devices/getAllRolesForDevice")]
    public async Task<ActionResult<GetAllRolesForDeviceResult>> GetAllRolesForDevice([FromBody] GetAllRolesForDeviceParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);

        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");
        var device = await deviceService.GetByIdAsync(parameters.DeviceId, cancellationToken);
        Check.Contract(device, $"Invalid {nameof(parameters.DeviceId)}");
        
        Check.Contract(device.OwnerId == auth.Owner.Id, $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.DeviceId)}");

        var roles = device.Roles.Select(r => new RoleResult
        {
            Id = r.Id,
            Name = r.Name
        });

        return Ok(new GetAllRolesForDeviceResult
            {
                Roles = roles
            }
        );
    }
    
    
    [HttpPost]
    [Route("devices/getAllDevices")]
    public async Task<ActionResult<GetAllDevicesResult>> GetAllRolesForDevice([FromBody] GetAllDevicesParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        
        var devicesFromDb = await deviceService.GetByOwnerIdAsync(auth.Owner.Id, cancellationToken);

        var devices = new GetAllDevicesResult
        {
            Devices = devicesFromDb.Select(device => new DeviceResult
            {
                Id = device.Id,
                Name = device.Name,
                Type = device.Type.ToString(),
                CreatedUtc = device.CreatedUtc,
                UpdatedUtc = device.UpdatedUtc
            })
        };

        return Ok(devices);
    }

    [HttpPost]
    [Route("devices/getOpeningKeys")]
    public async Task<ActionResult<GetOpeningKeysResult>> IssueNewKey([FromBody] GetOpeningKeysParameters parameters, CancellationToken cancellationToken)
    {
        var auth = await GetAuthorizationContextFromJwtTokenAsync(HttpContext, cancellationToken);
        
        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");
        var device = await deviceService.GetByIdAsync(parameters.DeviceId, cancellationToken);
        Check.Contract(device, $"Invalid {nameof(parameters.DeviceId)}");
        Check.Contract(device.OwnerId == auth.Owner.Id,
            $"Invalid {nameof(auth.Owner.Id)} and {nameof(parameters.DeviceId)}");
        
        var openingKeys = await deviceService.GetOpeningKeys(parameters.DeviceId, cancellationToken);
        Check.Contract(openingKeys, $"Invalid {nameof(parameters.DeviceId)}");

        var issueNewKeyResponse = new GetOpeningKeysResult
        {
            OpeningKeys = openingKeys.Select(k => new OpeningKey
            {
                KeyId = k.Id,
                Token = $"DevicePass_{k.Pass}{Guid.NewGuid()}"
            })
        };

        return Ok(issueNewKeyResponse);
    }
}