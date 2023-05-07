using Application.IRepositories;
using Application.Services;
using Application.Services.Devices;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext context;
    private readonly IDeviceService DeviceService;
    private readonly IDeviceRepository DeviceRepository;
    
    public AccessControlApi(
        AccessControlDbContext _context,
        IDeviceService _deviceService,
        IDeviceRepository _deviceRepository)
    {
        context = _context;
        DeviceService = _deviceService;
        DeviceRepository = _deviceRepository;
    }
}