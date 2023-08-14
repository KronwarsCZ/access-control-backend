using Application.IRepositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpGet]
    [Route("accounts/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> AccountsGetAll([FromBody] Dto.AccountFilterParameters parameters,
        CancellationToken cancellationToken)
    {
        var filters = new AccountFilters();

        var devices = await _accountService.GetAsync(filters, cancellationToken);

        return Ok(devices);
    }

    [HttpPost]
    [Route("accounts/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> AccountsAddAsync([FromBody] Dto.AccountAddParameters parameters,
        CancellationToken cancellationToken)
    {
        foreach (var user in parameters.Accounts) await _accountService.RegisterNewAccount(user.Email, user.Password, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
