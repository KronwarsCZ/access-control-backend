using Application.Services;
using Application.Services.DevicesRoles;
using Application.Services.Owner;
using Application.Services.RolesKeys;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext context;
    private readonly IDeviceService _deviceService;
    private readonly IOwnerService ownerService;
    private readonly IAccountService _accountService;
    private readonly IKeyService _keyService;
    private readonly IRoleService _roleService;
    private readonly IRolesKeysService _rolesKeysService;
    private readonly IDevicesRolesService _devicesRolesService;

    public AccessControlApi(
        AccessControlDbContext _context,
        IDeviceService deviceService,
        IOwnerService ownerService,
        IAccountService accountService,
        IKeyService keyService,
        IRoleService roleService,
        IRolesKeysService rolesKeysService,
        IDevicesRolesService devicesRolesService
        )
    {
        context = _context;
        _deviceService = deviceService;
        this.ownerService = ownerService;
        _accountService = accountService;
        _keyService = keyService;
        _roleService = roleService;
        _rolesKeysService = rolesKeysService;
        _devicesRolesService = devicesRolesService;
    }
}
