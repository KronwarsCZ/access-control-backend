using System.Collections.Immutable;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/getAll")]
    public ActionResult<IEnumerable<Device>> DevicesGetAll([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        Log.Debug("devices/getAll was called");
        
        return Ok(context.Devices.ToList());
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesAddAsync([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d =>
            DeviceService.Create(owner: d.Owner)
        ).ToImmutableArray();
        
        DeviceService.Add(devices);
        
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        
        Log.Debug("devices/add was called");
        
        return Ok(devices);
    }
}