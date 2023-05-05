using Application.Services;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

[Route("api/v1/")]
public partial class AccessControlApi : ControllerBase
{
    private readonly AccessControlDbContext context;
    
    public AccessControlApi(
        AccessControlDbContext context)
    {
        this.context = context;
    }
}