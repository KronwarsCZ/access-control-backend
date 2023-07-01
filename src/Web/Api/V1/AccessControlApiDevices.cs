using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAll([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        Log.Debug("devices/getAll was called");

        var filters = new DeviceFilters(parameters.Ids);
        var devices = await DeviceService.GetAsync(filters, cancellationToken);
        return Ok(devices);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesAddAsync([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d =>
            DeviceService.Create(owner: d.Owner)
        ).ToImmutableArray();
        
        await DeviceService.AddAsync(devices, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        
        Log.Debug("devices/add was called");
        
        return Ok(devices);
    }
}