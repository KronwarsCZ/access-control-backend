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
    public async Task<ActionResult<Guid>> AddDeviceToRoles([FromBody] Dto.DevicesRolesAddParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var owner = await GetFirstOwnerFromDb(cancellationToken);
        var device = _deviceService.Create(parameters.DevicesRoles.OneThing.Name, parameters.DevicesRoles.OneThing.Type, owner);
        var roles = parameters.DevicesRoles.ManyThings.Select(r => _roleService.Create(r.Name));
        
        var result = await _devicesRolesService.AddAtoBAsync(device, roles, cancellationToken);

        return Ok(result);
    }

    private async Task<Owner> GetFirstOwnerFromDb(CancellationToken cancellationToken)
    {
        var owners = await ownerService.GetAllAsync(cancellationToken);
       return owners.First();
    }

    //A is Role, B is Device
    [HttpPost]
    [Route("devicesroles/addRoleToDevices")]
    public async Task<ActionResult<Guid>> AddRoleToDevices(
        [FromBody] Dto.DevicesRolesAddParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var owner = await GetFirstOwnerFromDb(cancellationToken);

        var role = _roleService.Create(parameters.DevicesRoles.OneThing.Name);
        var devices = parameters.DevicesRoles.ManyThings.Select(d => _deviceService.Create(d.Name, d.Type, owner));
        
        var result = await _devicesRolesService.AddBtoAAsync(role, devices, cancellationToken);

        return Ok(result);
    }

    [HttpDelete]
    [Route("devicesroles/deleteDeviceToRoles")]
    public async Task<ActionResult> DeleteDeviceToRoles(
        [FromBody] Dto.DevicesRolesDeleteParameters<DeviceDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var owner = await GetFirstOwnerFromDb(cancellationToken);

        var device = _deviceService.Create(parameters.DevicesRoles.OneThing.Name, parameters.DevicesRoles.OneThing.Type, owner);
        var roles = parameters.DevicesRoles.ManyThings.Select(r => _roleService.Create(r.Name));

        await _devicesRolesService.DeleteAtoBAsync(device, roles, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("devicesroles/deleteRoleToDevices")]
    public async Task<ActionResult> DeleteRoleToDevices(
        [FromBody] Dto.DevicesRolesDeleteParameters<RoleDto, DeviceDto> parameters, CancellationToken cancellationToken)
    {
        var owner = await GetFirstOwnerFromDb(cancellationToken);

        var role = _roleService.Create(parameters.DevicesRoles.OneThing.Name);
        var devices = parameters.DevicesRoles.ManyThings.Select(d => _deviceService.Create(d.Name, d.Type, owner));

        await _devicesRolesService.DeleteBtoAAsync(role, devices, cancellationToken);

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
        //TODO: GetById...
        var roles = await _roleService.GetAllAsync(cancellationToken);
        var role = roles.First(r => r.Id == id);
        
        var keys = await _rolesKeysService.GetAtoBAsync(role, cancellationToken);

        return Ok(keys);
    }
    
    //A is Key, B is Role
    [HttpGet]
    [Route("roleskeys/getKeyToRoles")]
    public async Task<ActionResult<IEnumerable<Role>>> GetKeyToRolesAsync(Guid id,
        CancellationToken cancellationToken)
    {
        //TODO: GetById...
        var keys = await _keyService.GetAllAsync(cancellationToken);
        var key = keys.First(k => k.Id == id);
        
        var roles = await _rolesKeysService.GetBtoAAsync(key, cancellationToken);

        return Ok(roles);
    }
    
    //A is Role, B is Key
    [HttpPost]
    [Route("roleskeys/addRoleToKeys")]
    public async Task<ActionResult<Guid>> AddRoleToKeys(
        [FromBody] Dto.RolesKeysAddParameters<RoleDto, KeyDto> parameters, CancellationToken cancellationToken)
    {
        var role = _roleService.Create(parameters.RolesKeys.OneThing.Name);
        var keys = parameters.RolesKeys.ManyThings.Select(d => _keyService.Create(d.Name, d.Pass));
        
        var result = await _rolesKeysService.AddAtoBAsync(role, keys, cancellationToken);

        return Ok(result);
    }
    
    //A is Key, B is Role
    [HttpPost]
    [Route("roleskeys/addKeyToRoles")]
    public async Task<ActionResult<Guid>> AddKeyToRoles(
        [FromBody] Dto.RolesKeysAddParameters<KeyDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var key = _keyService.Create(parameters.RolesKeys.OneThing.Name, parameters.RolesKeys.OneThing.Pass);
        var roles = parameters.RolesKeys.ManyThings.Select(r => _roleService.Create(r.Name));
        
        var result = await _rolesKeysService.AddBtoAAsync(key, roles, cancellationToken);

        return Ok(result);
    }
    
    //A is Role, B is Key
    [HttpDelete]
    [Route("roleskeys/deleteRoleToKeys")]
    public async Task<ActionResult> DeleteRoleToKeys(
        [FromBody] Dto.RolesKeysDeleteParameters<RoleDto, KeyDto> parameters, CancellationToken cancellationToken)
    {
        var role = _roleService.Create(parameters.RolesKeys.OneThing.Name);
        var keys = parameters.RolesKeys.ManyThings.Select(d => _keyService.Create(d.Name, d.Pass));
        
        await _rolesKeysService.DeleteAtoBAsync(role, keys, cancellationToken);

        return Ok();
    }
    
    [HttpDelete]
    [Route("roleskeys/deleteKeyToRoles")]
    public async Task<ActionResult> DeleteKeyToRoles(
        [FromBody] Dto.RolesKeysDeleteParameters<KeyDto, RoleDto> parameters, CancellationToken cancellationToken)
    {
        var key = _keyService.Create(parameters.RolesKeys.OneThing.Name, parameters.RolesKeys.OneThing.Pass);
        var roles = parameters.RolesKeys.ManyThings.Select(r => _roleService.Create(r.Name));
        
        await _rolesKeysService.DeleteBtoAAsync(key, roles, cancellationToken);

        return Ok();
    }
    
    #endregion
}
