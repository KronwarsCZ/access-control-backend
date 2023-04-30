using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi : ControllerBase
{
    private readonly INecoService _service;

    public AccessControlApi(INecoService necoService)
    {
        _service = necoService;
    }
}