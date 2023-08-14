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

        return Ok(devices);
    }
    
    //A is Device, B is Role
    [HttpPost]
    [Route("devicesroles/addDeviceToRoles")]
    public async Task<ActionResult> AddDeviceToRoles(
        [FromBody] Dto.DevicesRolesAddParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var device = await _deviceService.GetByIdAsync(parameters.DevicesRoles.FirstThingGuid, cancellationToken);
        /*
        var roles = parameters.DevicesRoles.ManyThingsGuids
            .Select(async id => await _roleService.GetByIdAsync(id, cancellationToken));
        */
        //TODO: Zmenme getbyid v for loopu na getallbyids
        var roles = new List<Role>();
        foreach (var id in parameters.DevicesRoles.ManyThingsGuids)
        {
            roles.Add(await _roleService.GetByIdAsync(id, cancellationToken));
        }
        
        await _devicesRolesService.AddAtoBAsync(device, roles, cancellationToken);

        return Ok();
    }
    
    //A is Role, B is Device
    [HttpPost]
    [Route("devicesroles/addRoleToDevices")]
    public async Task<ActionResult> AddRoleToDevices(
        [FromBody] Dto.DevicesRolesAddParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(parameters.DevicesRoles.FirstThingGuid, cancellationToken);

        //TODO: Zmenme getbyid v for loopu na getallbyids
        var devices = new List<Device>();
        foreach (var id in parameters.DevicesRoles.ManyThingsGuids)
        {
            devices.Add(await _deviceService.GetByIdAsync(id, cancellationToken));
        }
        
        await _devicesRolesService.AddBtoAAsync(role, devices, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("devicesroles/deleteRolesFromDevice")]
    public async Task<ActionResult> DeleteRolesFromDevice(
        [FromBody] Dto.DevicesRolesDeleteParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var device = await _deviceService.GetByIdAsync(parameters.DevicesRoles.FirstThingGuid, cancellationToken);
        
        //TODO: Zmenme getbyid v for loopu na getallbyids
        var roles = new List<Role>();
        foreach (var id in parameters.DevicesRoles.ManyThingsGuids)
        {
            roles.Add(await _roleService.GetByIdAsync(id, cancellationToken));
        }
        
        await _devicesRolesService.DeleteBFromAAsync(device, roles, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("devicesroles/deleteDevicesFromRole")]
    public async Task<ActionResult> DeleteDevicesFromRole(
        [FromBody] Dto.DevicesRolesDeleteParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(parameters.DevicesRoles.FirstThingGuid, cancellationToken);
        
        //TODO: Zmenme getbyid v for loopu na getallbyids
        var devices = new List<Device>();
        foreach (var id in parameters.DevicesRoles.ManyThingsGuids)
        {
            devices.Add(await _deviceService.GetByIdAsync(id, cancellationToken));
        }
        
        await _devicesRolesService.DeleteAFromBAsync(role, devices, cancellationToken);

        return Ok();
    }

    #endregion

    #region RolesKeys

    //A is Role, B is Key
    [HttpGet]
    [Route("roleskeys/getRoleToKeys")]
    public async Task<ActionResult<IEnumerable<Key>>> GetRoleToKeysAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(id, cancellationToken);
        
        var keys = await _rolesKeysService.GetAtoBAsync(role, cancellationToken);

        return Ok(keys);
    }
    
    //A is Key, B is Role
    [HttpGet]
    [Route("roleskeys/getKeyToRoles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetKeyToRolesAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var key = await _keyService.GetByIdAsync(id, cancellationToken);
        
        var roles = await _rolesKeysService.GetBtoAAsync(key, cancellationToken);

        return Ok(roles);
    }
    
    //A is Role, B is Key
    [HttpPost]
    [Route("roleskeys/addRoleToKeys")]
    public async Task<ActionResult> AddRoleToKeys(
        [FromBody] Dto.RolesKeysAddParameters<RoleDto, KeyDto> parameters, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(parameters.RolesKeys.FirstThingGuid, cancellationToken);
        var keys = new List<Key>();
        foreach (var id in parameters.RolesKeys.ManyThingsGuids)
        {
            keys.Add(await _keyService.GetByIdAsync(id, cancellationToken));
        }
        
        await _rolesKeysService.AddAtoBAsync(role, keys, cancellationToken);

        return Ok();
    }
    
    //A is Key, B is Role
    [HttpPost]
    [Route("roleskeys/addKeyToRoles")]
    public async Task<ActionResult> AddKeyToRoles(
        [FromBody] Dto.RolesKeysAddParameters<KeyDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var key = await _keyService.GetByIdAsync(parameters.RolesKeys.FirstThingGuid, cancellationToken);
        var roles = new List<Role>();
        foreach (var id in parameters.RolesKeys.ManyThingsGuids)
        {
            roles.Add(await _roleService.GetByIdAsync(id, cancellationToken));
        }
        
        await _rolesKeysService.AddBtoAAsync(key, roles, cancellationToken);

        return Ok();
    }
    
    //A is Role, B is Key
    [HttpDelete]
    [Route("roleskeys/deleteKeysFromRole")]
    public async Task<ActionResult> DeleteKeysFromRole(
        [FromBody] Dto.RolesKeysDeleteParameters<RoleDto, KeyDto> parameters, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(parameters.RolesKeys.FirstThingGuid, cancellationToken);
        
        var keys = new List<Key>();
        foreach (var id in parameters.RolesKeys.ManyThingsGuids)
        {
            keys.Add(await _keyService.GetByIdAsync(id, cancellationToken));
        }
        
        await _rolesKeysService.DeleteBFromAAsync(role, keys, cancellationToken);

        return Ok();
    }
    
    [HttpDelete]
    [Route("roleskeys/deleteRolesFromKey")]
    public async Task<ActionResult> DeleteRolesFromKey(
        [FromBody] Dto.RolesKeysDeleteParameters<KeyDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var key = await _keyService.GetByIdAsync(parameters.RolesKeys.FirstThingGuid, cancellationToken);
        
        var roles = new List<Role>();
        foreach (var id in parameters.RolesKeys.ManyThingsGuids)
        {
            roles.Add(await _roleService.GetByIdAsync(id, cancellationToken));
        }        
        
        await _rolesKeysService.DeleteAFromBAsync(key, roles, cancellationToken);

        return Ok();
    }
    
    #endregion
}
