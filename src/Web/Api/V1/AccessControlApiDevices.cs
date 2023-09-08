using Core.Entities;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/registerNewDevice")]
    public async Task<ActionResult<Dto.DeviceAddResult>> AddDevices([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        Check.Contract(!string.IsNullOrWhiteSpace(parameters.Name), $"{nameof(parameters.Name)} cannot be empty.");
        Check.Contract(parameters.OwnerId != Guid.Empty, $"Invalid {nameof(parameters.OwnerId)}");
        var owner = await ownerService.GetByIdAsync(parameters.OwnerId, cancellationToken);
        if (owner is null) BadRequest();

        var newDevice = Device.Create(parameters.Name);
        await deviceService.RegisterNewDevice(owner, newDevice, cancellationToken);

        return Ok(new Dto.DeviceAddResult { DeviceId = newDevice.Id, Type = newDevice.Type.ToString()});
    }
}