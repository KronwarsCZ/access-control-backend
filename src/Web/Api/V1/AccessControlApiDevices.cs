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
        var deviceFilters = new DeviceFilters();
        // var devices = DeviceService.GetAsync(deviceFilters);
        
        var a = context.Devices.ToList();
        
        return Ok(a);
    }

    [HttpPost]
    [Route("devices/add")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesAddAsync([FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        var devices = parameters.Devices.Select(d =>
            DeviceService.Create(owner: d.Owner)
        ).ToList();
        DeviceRepository.Add(devices);
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        
        return Ok(devices);
    }
}