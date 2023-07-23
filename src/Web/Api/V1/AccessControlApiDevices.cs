using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("devices/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAll([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        var filters = new DeviceFilters();
        
        var devices = await DeviceService.GetAsync(filters, cancellationToken);
        
        return Ok(devices);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> DevicesAddAsync([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d =>
            DeviceService.Create(owner: d.Owner)
        ).ToImmutableArray();
        
        var ids = DeviceService.Add(devices);
        
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        
        return Ok(ids);
    }
}