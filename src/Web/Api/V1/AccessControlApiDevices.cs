using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAllAsync([FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        //var filters = new DeviceFilters();
        
        var devices = await _deviceService.GetAllAsync(cancellationToken);
        
        if (devices is null) return NoContent();
        return Ok(devices);
    }
    
    [HttpDelete]
    [Route("devices/deleteRoles")]
    public async Task<ActionResult<(int, IEnumerable<string>)>> DeleteDevicesAsync(
        [FromBody] Dto.DeviceFilterParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters?.Ids is null)
        {
            return UnprocessableEntity();
        }

        var result = await _deviceService.DeleteAsync(parameters.Ids, cancellationToken);

        if (result is { Item1: 0, Item2: null }) return BadRequest();

        return Ok(result.Item1);
    }
    
    [HttpPut]
    [Route("devices/updateDevice")]
    public async Task<ActionResult<Key>> UpdateDeviceAsync(Guid id, [FromBody] Dto.DeviceAddParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters is null) return BadRequest();

        var deviceFromParameters = parameters.Devices.First();

        var role = _deviceService.Create(deviceFromParameters.Id, deviceFromParameters.Name, deviceFromParameters.IsDeleted, deviceFromParameters.Type);
        
        var result = await _deviceService.UpdateAsync(id, role, cancellationToken);

        if (result is null) return BadRequest();
        return Ok(result);
    }
    
    
    [HttpPost]
    [Route("devices/addDevices")]
    public async Task<ActionResult<IEnumerable<Key>>> AddDevices( [FromBody] Dto.DeviceAddParameters parameters,
        CancellationToken cancellationToken)
    {
        if (parameters is null || !parameters.Devices.Any())
        {
            return BadRequest();
        }

        var devices = parameters.Devices
            .Select(d => _deviceService.Create(d.Id, d.Name, d.IsDeleted, d.Type));
        
        var result = await _deviceService.AddAsync(devices, cancellationToken);

        return result is null ? Problem() : Ok(result);
    }
}