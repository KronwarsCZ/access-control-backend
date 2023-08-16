using Application.Services;
using Application.Services.DevicesRoles;
using Application.Services.Owner;
using Application.Services.RolesKeys;
using Core.Entities;
using Infrastructure.Database;

namespace ApiIntegrationTests;

public partial class SampleDataService
{
    private readonly AccessControlDbContext context;
    private readonly IDeviceService _deviceService;
    private readonly IOwnerService ownerService;
    private readonly IAccountService _accountService;
    private readonly IKeyService _keyService;
    private readonly IRoleService _roleService;
    private readonly IRolesKeysService _rolesKeysService;
    private readonly IDevicesRolesService _devicesRolesService;
    public SampleDataService(
        AccessControlDbContext _context,
        IDeviceService deviceService,
        IOwnerService ownerService,
        IAccountService accountService,
        IKeyService keyService,
        IRoleService roleService,
        IRolesKeysService rolesKeysService,
        IDevicesRolesService devicesRolesService)
    {
        context = _context;
        _deviceService = deviceService;
        this.ownerService = ownerService;
        _accountService = accountService;
        _keyService = keyService;
        _roleService = roleService;
        _rolesKeysService = rolesKeysService;
        _devicesRolesService = devicesRolesService;
        
        SampleOwner = ownerService.Create("Sample Owner");
    }
    
    private readonly Owner SampleOwner;

    public Device Device(DeviceType type, Owner owner = null, string name = null)
    {
        return _deviceService.Create(name ?? "random name of device", type, owner ?? SampleOwner);
    }
}