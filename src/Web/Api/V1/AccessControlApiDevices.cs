using AccessControl.Web.Api.V1.Dto;
using Core.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/getAll")]
    public ActionResult<IEnumerable<Device>> DevicesGetAll([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        var a = Context.Devices.ToList();
        return Ok(a);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesAddAsync([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d => new Device
        {
            Owner = d.Owner
        });
        foreach (var device in devices)
        {
            await Context.AddAsync(device, cancellationToken);
        }
        await Context.SaveChangesAsync(cancellationToken: cancellationToken);
        return Ok(devices);
    }
}