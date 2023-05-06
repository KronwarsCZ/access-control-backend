using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public ActionResult<IEnumerable<Device>> DevicesGetAll(Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        var a = context.Devices.ToList();
        return Ok(a);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<Device>> DevicesAddAsync(Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d => new Device
        {
            Owner = d.Owner
        });

        foreach (var device in devices)
        {
            await context.Devices.AddAsync(device, cancellationToken);
        }
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        return Ok(devices);
    }
}