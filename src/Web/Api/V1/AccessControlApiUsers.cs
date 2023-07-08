using System.Collections.Immutable;
using Application.IRepositories;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("users/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAll([FromBody] Dto.UserFilterParameters parameters,
        CancellationToken cancellationToken)
    {
        var filters = new UserFilters();

        var devices = await UserService.GetAsync(filters, cancellationToken);

        return Ok(devices);
    }

    [HttpPost]
    [Route("users/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> DevicesAddAsync([FromBody] Dto.UserAddParameters parameters,
        CancellationToken cancellationToken)
    {
        foreach (var user in parameters.Users) UserService.RegisterNewUser(user.Email, user.Password);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
