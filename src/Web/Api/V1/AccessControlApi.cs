using Application.Services.Devices;
using Application.Services.DevicesRoles;
using Application.Services.Keys;
using Application.Services.Owners;
using Application.Services.Roles;
using Application.Services.RolesKeys;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly IDeviceService deviceService;
    private readonly IOwnerService ownerService;
    private readonly IKeyService keyService;
    private readonly IRoleService roleService;
    private readonly IRolesKeysService rolesKeysService;
    private readonly IDevicesRolesService devicesRolesService;

    public AccessControlApi(
        IDeviceService deviceService,
        IOwnerService ownerService,
        IKeyService keyService,
        IRoleService roleService,
        IRolesKeysService rolesKeysService,
        IDevicesRolesService devicesRolesService)
    {
        this.deviceService = deviceService;
        this.ownerService = ownerService;
        this.keyService = keyService;
        this.roleService = roleService;
        this.rolesKeysService = rolesKeysService;
        this.devicesRolesService = devicesRolesService;
    }
}
