using System.Collections.Immutable;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAllAsync(CancellationToken cancellationToken)
    {
        //var filters = new DeviceFilters();

        var devices = await _deviceService.GetAllAsync(cancellationToken);

        return Ok(devices);
    }
    
    [HttpDelete]
    [Route("devices/deleteDevices")]
    public async Task<ActionResult> DeleteDevicesAsync(
        [FromBody] Dto.DeviceDeleteParameters parameters, CancellationToken cancellationToken)
    {
        //TODO: Prepsat na pouziti GetAllById
        var allIds = parameters.Ids;
        var devices = await _deviceService.GetAllAsync(cancellationToken);
        var devicesToDelete = devices.Where(d => allIds.Contains(d.Id)).ToImmutableArray();

        await _deviceService.Delete(devicesToDelete, cancellationToken);

        return Ok();
    }
    
    [HttpPut]
    [Route("devices/updateDevice")]
    public async Task<ActionResult> UpdateDeviceAsync([FromBody] Dto.DeviceUpdateParameters parameters, CancellationToken cancellationToken)
    {
        //TODO
        await _deviceService.Update(null, cancellationToken);

        return Ok();
    }
    
    
    [HttpPost]
    [Route("devices/addDevices")]
    public async Task<ActionResult<IEnumerable<Guid>>> AddDevices( [FromBody] Dto.DeviceAddParameters parameters,
        CancellationToken cancellationToken)
    {
        var owner = await GetFirstOwnerFromDb(cancellationToken);

       var devices = parameters.Devices
            .Select(d => _deviceService.Create(d.Name, d.Type, owner)).ToImmutableArray();
        
        var result = await _deviceService.AddAsync(devices, cancellationToken);

        return Ok(result);
    }
}