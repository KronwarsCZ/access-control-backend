using Application.IRepositories;
using Application.Services;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext context;
    private readonly IDeviceService DeviceService;
    private readonly IAccountService _accountService;
    private readonly IKeyService _keyService;

    public AccessControlApi(
        AccessControlDbContext _context,
        IDeviceService _deviceService,
        IAccountService accountService,
        IKeyService keyService)
    {
        context = _context;
        DeviceService = _deviceService;
        _accountService = accountService;
        _keyService = keyService;
    }
}
