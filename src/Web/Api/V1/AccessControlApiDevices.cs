using AccessControl.Web.Api.V1.Dto;
using AccessControl.Web.Api.V1.Dto.Devices;
using Core.Entities.Devices;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/registerNewDevice")]
    public async Task<ActionResult<DeviceAddResult>> AddDevices([FromBody] DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        if (owner is null) BadRequest();
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");

        var newDevice = Device.Create(owner, parameters.Name, DeviceType.Lock);
        await deviceService.RegisterNewDevice(newDevice, cancellationToken);

        return Ok(new DeviceAddResult { DeviceId = newDevice.Id, Type = newDevice.Type.ToString()});
    }

    [HttpPost]
    [Route("devices/attachRoleToDevice")]
    public async Task<ActionResult> AttachRoleToDevice([FromBody] AttachRoleToDeviceParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        Check.Contract(owner, $"Invalid {nameof(parameters.OwnerId)}");

        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");    
        Check.Contract(parameters.RoleId != Guid.Empty, $"Invalid {nameof(parameters.RoleId)}");

        var device = await deviceService.GetByIdAsync(parameters.DeviceId, cancellationToken);
        Check.Contract(device.OwnerId == owner.Id, $"Invalid {nameof(parameters.DeviceId)}");
        
        var role = await roleService.GetByIdAsync(parameters.RoleId, cancellationToken);
        Check.Contract(role.OwnerId == owner.Id, $"Invalid {nameof(parameters.RoleId)}");

        await deviceService.AttachRoleToDevice(device, role, cancellationToken);

        return Ok();
    }
    
    [HttpPost]
    [Route("devices/getAllRolesForDevice")]
    public async Task<ActionResult<Dto.GetAllRolesForDeviceResult>> GetAllRolesForDevice([FromBody] GetAllRolesForDeviceParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        Check.Contract(owner, $"Invalid {nameof(parameters.OwnerId)}");

        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");

        var device = await deviceService.GetByIdAsync(parameters.DeviceId, cancellationToken);
        Check.Contract(device.OwnerId == owner.Id, $"Invalid {nameof(parameters.DeviceId)}");

        var roles = device.Roles.Select(r => new RoleResult
        {
            Id = r.Id,
            Name = r.Name
        });

        return Ok(new GetAllRolesForDeviceResult
        {
            Roles = roles
        });
    }
    
    [HttpPost]
    [Route("devices/getOpeningKeys")]
    public async Task<ActionResult<GetOpeningKeysResult>> IssueNewKey([FromBody] GetOpeningKeysParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        Check.Contract(owner, $"Invalid {nameof(parameters.OwnerId)}");

        
        Check.Contract(parameters.DeviceId != Guid.Empty, $"Invalid {nameof(parameters.DeviceId)}");
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