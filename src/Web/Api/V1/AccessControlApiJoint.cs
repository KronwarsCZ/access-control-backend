using AccessControl.Web.Api.V1.Dto;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    #region DevicesRoles
    
    //A is Device, B is Role
    [HttpGet]
    [Route("devicesroles/getDeviceToRoles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetDeviceToRolesAsync(Guid id,
        CancellationToken cancellationToken)
    {
        //TODO: GetById...
        var devices = await _deviceService.GetAllAsync(cancellationToken);
        var device = devices.First(d => d.Id == id);
        
        var roles = await _devicesRolesService.GetAtoBAsync(device, cancellationToken);

        return Ok(roles);
    }
    
    //A is Role, B is Device
    [HttpGet]
    [Route("devicesroles/getRoleToDevices")]
    public async Task<ActionResult<IEnumerable<Device>>> GetRoleToDevicesAsync(Guid id,
        CancellationToken cancellationToken)
    {
        //TODO: GetById...
        var roles = await _roleService.GetAllAsync(cancellationToken);
        var role = roles.First(d => d.Id == id);
        
        var devices = await _devicesRolesService.GetBtoAAsync(role, cancellationToken);

        return Ok(roles);
    }
    
    //A is Device, B is Role
    [HttpPost]
    [Route("devicesroles/addDeviceToRoles")]
    public async Task<ActionResult<Guid>> AddDeviceToRoles(
        [FromBody] Dto.DevicesRolesAddParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var device = _deviceService.Create(parameters.DevicesRoles.oneThing.Name, parameters.DevicesRoles.oneThing.Type);
        var roles = parameters.DevicesRoles.manyThings.Select(r => _roleService.Create(r.Name));
        
        var result = await _devicesRolesService.AddAtoBAsync(device, roles, cancellationToken);

        return Ok(result);
    }
    
    //A is Role, B is Device
    [HttpPost]
    [Route("devicesroles/addRoleToDevices")]
    public async Task<ActionResult<Guid>> AddRoleToDevices(
        [FromBody] Dto.DevicesRolesAddParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var role = _roleService.Create(parameters.DevicesRoles.oneThing.Name);
        var devices = parameters.DevicesRoles.manyThings.Select(d => _deviceService.Create(d.Name, d.Type));
        
        var result = await _devicesRolesService.AddBtoAAsync(role, devices, cancellationToken);

        return Ok(result);
    }

    [HttpDelete]
    [Route("devicesroles/deleteDeviceToRoles")]
    public async Task<ActionResult> DeleteDeviceToRoles(
        [FromBody] Dto.DevicesRolesDeleteParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var device = _deviceService.Create(parameters.DevicesRoles.oneThing.Name, parameters.DevicesRoles.oneThing.Type);
        var roles = parameters.DevicesRoles.manyThings.Select(r => _roleService.Create(r.Name));

        await _devicesRolesService.DeleteAtoBAsync(device, roles, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("devicesroles/deleteRoleToDevices")]
    public async Task<ActionResult> DeleteRoleToDevices(
        [FromBody] Dto.DevicesRolesDeleteParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var role = _roleService.Create(parameters.DevicesRoles.oneThing.Name);
        var devices = parameters.DevicesRoles.manyThings.Select(d => _deviceService.Create(d.Name, d.Type));

        await _devicesRolesService.DeleteBtoAAsync(role, devices, cancellationToken);

        return Ok();
    }

    #endregion
    
}
