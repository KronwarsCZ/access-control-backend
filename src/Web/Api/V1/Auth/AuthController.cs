using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiShared.Authorization;
using ApiShared.Dto.Auth;
using Application.Services.Accounts;
using Application.Services.Owners;
using Application.Services.Time;
using Framework.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Web.Api.V1.Auth;

[Route("api/v1/")]
public class AuthApi : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly ITimeService _timeService;
    private readonly IOwnerService ownerService;

    public AuthApi(
        IAccountService accountService,
        IOwnerService ownerService,
        TimeFreezeService timeService)
    {
        this.accountService = accountService;
        this.ownerService = ownerService;
        _timeService = timeService;
    }


}