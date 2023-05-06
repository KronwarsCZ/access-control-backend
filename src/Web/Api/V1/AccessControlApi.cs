using Application.Services;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext context;
    private readonly DeviceService _deviceService;
    
    public AccessControlApi(
        AccessControlDbContext context)
    {
        this.context = context;
    }
}