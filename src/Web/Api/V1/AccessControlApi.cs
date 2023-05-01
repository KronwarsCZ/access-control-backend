using Application.Services;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi : ControllerBase
{
    private readonly INecoService _service;
    private readonly AccessControlDbContext context;
    
    public AccessControlApi(INecoService necoService, AccessControlDbContext context)
    {
        this.context = context;
        _service = necoService;
    }
}