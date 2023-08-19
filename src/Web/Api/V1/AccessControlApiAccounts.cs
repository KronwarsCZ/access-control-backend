using AccessControl.Web.Api.V1.Dto.Accounts;
using Application.IRepositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Web.Api.V1;

public partial class AccessControlApi
{
    [HttpPost]
    [Route("accounts/getAll")]
    public async Task<ActionResult<IEnumerable<Device>>> AccountsGetAll([FromBody] AccountFilterParameters parameters,
        CancellationToken cancellationToken)
    {
        var filters = new AccountFilters();

        var devices = await _accountService.GetAccountByEmailAsync("string", cancellationToken);

        return Ok(devices);
    }

    [HttpPost]
    [Route("accounts/add")]
    public async Task<ActionResult<IEnumerable<Guid>>> AccountsAddAsync([FromBody] AccountAddParameters parameters,
        CancellationToken cancellationToken)
    {
        foreach (var user in parameters.Accounts) await _accountService.RegisterNewAccount(user.Email, user.Password, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
