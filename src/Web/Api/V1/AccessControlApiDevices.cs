using AccessControl.Web.Api.V1.Dto.Devices;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("devices/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAllAsync(CancellationToken cancellationToken)
    {
        var devices = await deviceService.GetAllAsync(cancellationToken);
        
        return Ok(devices);
    }
    
    [HttpDelete]
    [Route("devices/deleteDevices")]
    public async Task<ActionResult> DeleteDevicesAsync(
        [FromBody] DeviceDeleteParameters parameters, CancellationToken cancellationToken)
    {
        
        return Ok();
    }
    
    [HttpPut]
    [Route("devices/updateDevice")]
    public async Task<ActionResult> UpdateDeviceAsync([FromBody] DeviceUpdateParameters parameters, CancellationToken cancellationToken)
    {

        return Ok();
    }
    
    
    [HttpPost]
    [Route("devices/addDevices")]
    public async Task<ActionResult<IEnumerable<Guid>>> AddDevices( [FromBody] DeviceAddParameters parameters, CancellationToken cancellationToken)
    {


        return Ok();
    }
}