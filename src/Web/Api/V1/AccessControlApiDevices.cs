using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public ActionResult<IEnumerable<Device>> DevicesGetAll()
    {
        var a = context.Devices.ToList();
        return Ok(a);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<Device>> DevicesAddAsync(CancellationToken cancellationToken)
    {
        var device = new Device
        {
            Owner = "nejaky owner",
        }; 
        await context.Devices.AddAsync(device, cancellationToken);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        return Ok(device);
    }
}