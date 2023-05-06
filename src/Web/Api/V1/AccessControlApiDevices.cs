using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public ActionResult<IEnumerable<Device>> DevicesGetAll([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        var id = parameters.Ids.First();
        var a = context.Devices.Where(d => d.Id == id).ToList();
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
        _deviceService.ContractedProcessMethod();
        foreach (var device in devices)
        {
            await context.AddAsync(device, cancellationToken);
        }
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        return Ok(devices);
    }
}