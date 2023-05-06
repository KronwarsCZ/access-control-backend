using Application.Services;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext Context;
    private readonly IDeviceService DeviceService;
    
    public AccessControlApi(
        AccessControlDbContext _context,
        IDeviceService _deviceService)
    {
        Context = _context;
        DeviceService = _deviceService;
    }
}