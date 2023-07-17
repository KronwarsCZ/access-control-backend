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
    [Route("accounts/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> DevicesGetAll([FromBody] Dto.AccountFilterParameters parameters,
        CancellationToken cancellationToken)
    {
        var filters = new AccountFilters();

        var devices = await _accountService.GetAsync(filters, cancellationToken);

        return Ok(devices);
    }

    [HttpPost]
    [Route("accounts/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> DevicesAddAsync([FromBody] Dto.AccountAddParameters parameters,
        CancellationToken cancellationToken)
    {
        foreach (var user in parameters.Accounts) _accountService.RegisterNewAccount(user.Email, user.Password);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
